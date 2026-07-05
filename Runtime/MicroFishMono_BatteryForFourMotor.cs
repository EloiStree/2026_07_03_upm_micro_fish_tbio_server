using System;
using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_BatteryForFourMotor : MonoBehaviour
{

    public UnityEvent<bool> m_onBatteryHasEnoughPower;
    public UnityEvent m_onBatteryIsEmpty;


    [Header("LiPo Battery Specs (Single Cell)")]
    [Tooltip("Total capacity in mAh")]
    public float m_batteryCapacityMilliAmperHour = 600f;

    [Tooltip("Voltage when fully charged (4.2V for LiPo)")]
    public float m_fullVoltage = 4.2f;

    [Tooltip("Voltage where battery is empty (3.0V for LiPo)")]
    public float m_emptyVoltage = 3.0f;

    [Tooltip("Internal resistance in Ohms - causes voltage drop under load")]
    public float m_internalResistanceOhm = 0.15f;

    [Header("Current Draw")]
    [Tooltip("Current drawn by ONE motor at 100% power (Amps)")]
    public float m_motorRatedCurrentAmp = 0.12f;

    [Tooltip("System idle current when all motors off (Amps)")]
    public float m_idleCurrentAmp = 0.05f;

    [Header("Motor Intensities (0 to 1)")]
    [Range(0, 1)] public float m_motorIntensityLeft;
    [Range(0, 1)] public float m_motorIntensityRight;
    [Range(0, 1)] public float m_motorIntensityBack;
    [Range(0, 1)] public float m_motorIntensityFront;

    [Header("Runtime (Read Only)")]
    public float m_batteryRemainingMilliAmperHour;
    public float m_totalCurrentAmp;
    public float m_totalPowerWatt;
    public float m_consumedMilliAmpereHour;
    public float m_consumedWattHour;
    public float m_openCircuitVoltage;   // Voltage without load
    public float m_loadVoltage;          // Voltage motors actually see
    public float m_voltageSag;           // How much voltage drops under load
    public float m_batteryPercent;
    public bool m_isBatteryEmpty;
    public float m_estimatedMinutesLeft;

    [System.Serializable]
    public class BatteryEvents
    {
        public UnityEvent<bool> m_onBatteryEmptyChanged;
        public UnityEvent<float> m_onVoltageUpdated;
        public UnityEvent<float> m_onPercentUpdated;
        public UnityEvent<float> m_onCurrentUpdated;
    }

    public BatteryEvents m_events = new BatteryEvents();

    private bool m_previousBatteryEmpty;
    private float m_previousVoltage = -1f;
    private float m_previousPercent = -1f;
    private float m_previousCurrent = -1f;

    void Awake()
    {
        m_batteryRemainingMilliAmperHour = m_batteryCapacityMilliAmperHour;
        UpdateBatteryState(true);
        m_onBatteryHasEnoughPower.Invoke(!m_isBatteryEmpty);
    }

    /// <summary>
    /// Set all 4 motor intensities (values clamped to 0-1)
    /// </summary>
    public void SetMotorPercent(float left, float right, float back, float front)
    {
        m_motorIntensityLeft = Mathf.Clamp01(Mathf.Abs(left));
        m_motorIntensityRight = Mathf.Clamp01(Mathf.Abs(right));
        m_motorIntensityBack = Mathf.Clamp01(Mathf.Abs(back));
        m_motorIntensityFront = Mathf.Clamp01(Mathf.Abs(front));
    }

    void Update()
    {
        // When empty, no current flows
        if (m_isBatteryEmpty)
        {
            m_totalCurrentAmp = 0f;
            m_totalPowerWatt = 0f;
            m_loadVoltage = 0f;
            m_voltageSag = 0f;
            m_estimatedMinutesLeft = 0f;
            UpdateBatteryState(false);
            return;
        }

        // === 1. Calculate Total Current Draw ===
        // Sum of all motors + system idle
        float motorSum = m_motorIntensityLeft + m_motorIntensityRight +
                         m_motorIntensityBack + m_motorIntensityFront;

        m_totalCurrentAmp = (motorSum * m_motorRatedCurrentAmp) + m_idleCurrentAmp;

        // === 2. Calculate Open Circuit Voltage (no load) ===
        // This is the "true" battery voltage based on remaining capacity
        m_openCircuitVoltage = CalculateLiPoVoltage(m_batteryPercent);

        // === 3. Calculate Voltage Under Load (voltage sag) ===
        // V_load = V_open - (I × R_internal)
        // This is why batteries voltage drops when you throttle up!
        m_voltageSag = m_totalCurrentAmp * m_internalResistanceOhm;
        m_loadVoltage = Mathf.Max(0f, m_openCircuitVoltage - m_voltageSag);

        // === 4. Calculate Power (using ACTUAL voltage under load) ===
        // P = V × I (Watts)
        m_totalPowerWatt = m_loadVoltage * m_totalCurrentAmp;

        // === 5. Drain Battery ===
        // mAh = Amps × 1000 × (seconds / 3600)
        float consumedThisFrame = m_totalCurrentAmp * 1000f * Time.deltaTime / 3600f;
        m_batteryRemainingMilliAmperHour = Mathf.Max(0f, m_batteryRemainingMilliAmperHour - consumedThisFrame);

        // === 6. Track Total Consumption ===
        m_consumedMilliAmpereHour += consumedThisFrame;
        m_consumedWattHour += m_totalPowerWatt * Time.deltaTime / 3600f;

        // === 7. Estimate Time Remaining ===
        m_estimatedMinutesLeft = m_totalCurrentAmp > 0.001f
            ? (m_batteryRemainingMilliAmperHour / (m_totalCurrentAmp * 1000f)) * 60f
            : float.PositiveInfinity;

        UpdateBatteryState(false);
    }

    /// <summary>
    /// Simulates realistic LiPo discharge curve.
    /// 
    /// Real LiPo behavior:
    /// - Quick drop from 4.2V → ~3.9V in first 10-15%
    /// - Relatively flat around 3.7V for middle 50-60%
    /// - Drops faster again at the end toward 3.0V
    /// </summary>
    private float CalculateLiPoVoltage(float percent)
    {
        float p = Mathf.Clamp01(percent);
        float range = m_fullVoltage - m_emptyVoltage; // ~1.2V

        // Blend curves to approximate LiPo shape:
        // - Low power: drops fast at start (simulates initial 4.2→3.9V drop)
        // - Linear: flat middle section
        // - High power: drops fast at end (simulates final drop to 3.0V)
        float fastStart = Mathf.Pow(p, 0.25f);  // Emphasizes initial drop
        float flat = p;                          // Linear middle
        float fastEnd = Mathf.Pow(p, 2.5f);     // Emphasizes final drop

        // Weighted blend (more weight on flat middle)
        float curve = 0.15f * fastStart + 0.65f * flat + 0.20f * fastEnd;

        return m_emptyVoltage + range * curve;
    }
    private void UpdateBatteryState(bool force)
    {
        // Calculate percentage from remaining capacity
        m_batteryPercent = Mathf.Clamp01(m_batteryRemainingMilliAmperHour / m_batteryCapacityMilliAmperHour);

        // Battery is empty when capacity is depleted
        // (NOT based on voltage, to avoid circular logic)
        bool previousEmptyState = m_isBatteryEmpty;
        m_isBatteryEmpty = m_batteryRemainingMilliAmperHour <= 0.01f;

        if (previousEmptyState != m_isBatteryEmpty)
        {
            m_onBatteryHasEnoughPower.Invoke(!m_isBatteryEmpty);
            if (m_isBatteryEmpty)
                m_onBatteryIsEmpty.Invoke();
        }

        // Fire events only when values change significantly
        if (force || Mathf.Abs(m_previousCurrent - m_totalCurrentAmp) > 0.001f)
        {
            m_previousCurrent = m_totalCurrentAmp;
            m_events.m_onCurrentUpdated?.Invoke(m_totalCurrentAmp);
        }

        if (force || Mathf.Abs(m_previousVoltage - m_loadVoltage) > 0.001f)
        {
            m_previousVoltage = m_loadVoltage;
            m_events.m_onVoltageUpdated?.Invoke(m_loadVoltage);
        }

        if (force || Mathf.Abs(m_previousPercent - m_batteryPercent) > 0.001f)
        {
            m_previousPercent = m_batteryPercent;
            m_events.m_onPercentUpdated?.Invoke(m_batteryPercent);
        }

        if (force || m_previousBatteryEmpty != m_isBatteryEmpty)
        {
            m_previousBatteryEmpty = m_isBatteryEmpty;
            m_events.m_onBatteryEmptyChanged?.Invoke(m_isBatteryEmpty);
        }
    }

    /// <summary>
    /// Fully recharge the battery
    /// </summary>
    public void Recharge()
    {
        m_batteryRemainingMilliAmperHour = m_batteryCapacityMilliAmperHour;
        m_consumedMilliAmpereHour = 0f;
        m_consumedWattHour = 0f;
        m_totalCurrentAmp = 0f;
        m_totalPowerWatt = 0f;
        UpdateBatteryState(true);
    }

    /// <summary>
    /// Set battery to a specific percentage (for testing)
    /// </summary>
    public void SetBatteryPercent(float percent)
    {
        m_batteryRemainingMilliAmperHour = m_batteryCapacityMilliAmperHour * Mathf.Clamp01(percent);
        UpdateBatteryState(true);
    }

    public float GetBatteryPercent()
    {
        return m_batteryPercent;
    }
}