using UnityEngine;


namespace Eloi.MicroFish
{
    /// <summary>
    /// Submarine composed of two back motor left and right.
    /// And two vertical motor back and front.
    /// 
    /// Motor Mapping:
    /// - Left/Right Motors: Control forward speed and yaw (turning)
    /// - Front/Back Vertical Motors: Control pitch and vertical movement
    /// </summary>
    public class MicroFishMono_BasicTransform : MonoBehaviour
    {
        public Transform m_whatToMove;

        [Header("Motor Position (Visual)")]
        public Transform m_leftMotor;
        public Transform m_rightMotor;
        public Transform m_backTopMotor;
        public Transform m_frontTopMotor;

        [Header("Motor Pivot")]
        public Transform m_backFrontMotorPivot;
        public Transform m_leftRightMotorPivot;

        [Header("Motor Percentages")]
        [Range(-1f, 1f)] public float m_leftMotorPercent11 = 0f;
        [Range(-1f, 1f)] public float m_rightMotorPercent11 = 0f;
        [Range(-1f, 1f)] public float m_backMotorPercent11 = 0f;
        [Range(-1f, 1f)] public float m_frontMotorPercent11 = 0f;

        [Header("Computed Movement (Read Only)")]
        public float m_computedForward = 0f;
        public float m_computedYaw = 0f;
        public float m_computedPitch = 0f;
        public float m_computedVertical = 0f;

        [Header("Speed Settings")]
        public float m_forwardSpeedMax = 1f;
        public float m_yawSpeedMax = 90f;
        public float m_pitchSpeedMax = 45f;
        public float m_verticalSpeedMax = 0.5f;
        public float m_pitchAngleMax = 35f;

        [Header("Water Resistance (Damping)")]
        public float m_linearDamping = 2f;
        public float m_angularDamping = 3f;

        [Header("Visual Settings")]
        public float m_propellerSpinSpeed = 720f;

        // Internal state
        private float m_currentYawAngle = 0f;
        private float m_currentPitchAngle = 0f;
        private float m_currentForwardVelocity = 0f;
        private float m_currentVerticalVelocity = 0f;

        public void Update()
        {
            // Convert motor values to movement values
            MotorToMovementInput();

            // Apply water resistance (damping)
            m_currentForwardVelocity = Mathf.Lerp(
                m_currentForwardVelocity,
                m_computedForward * m_forwardSpeedMax,
                m_linearDamping * Time.deltaTime
            );
            m_currentVerticalVelocity = Mathf.Lerp(
                m_currentVerticalVelocity,
                m_computedVertical * m_verticalSpeedMax,
                m_linearDamping * Time.deltaTime
            );

            // Apply yaw rotation with damping
            float targetYawDelta = m_computedYaw * m_yawSpeedMax * Time.deltaTime;
            m_currentYawAngle += targetYawDelta;

            // Apply pitch rotation with damping and clamping
            float targetPitchDelta = m_computedPitch * m_pitchSpeedMax * Time.deltaTime;
            m_currentPitchAngle += targetPitchDelta;
            m_currentPitchAngle = Mathf.Clamp(m_currentPitchAngle, -m_pitchAngleMax, m_pitchAngleMax);

            // Return pitch to neutral when no input
            if (Mathf.Abs(m_computedPitch) < 0.01f)
            {
                m_currentPitchAngle = Mathf.Lerp(m_currentPitchAngle, 0f, m_angularDamping * Time.deltaTime);
            }

            // Apply rotation
            m_whatToMove.rotation = Quaternion.Euler(m_currentPitchAngle, m_currentYawAngle, 0f);

            // Apply movement in local space (forward)
            m_whatToMove.Translate(Vector3.forward * m_currentForwardVelocity * Time.deltaTime, Space.Self);

            // Apply vertical movement in world space (up/down)
            m_whatToMove.Translate(Vector3.up * m_currentVerticalVelocity * Time.deltaTime, Space.World);

            // Update motor visuals
            UpdateMotorVisuals();
        }

        /// <summary>
        /// Converts individual motor percentages to combined movement values.
        /// 
        /// Physics Logic:
        /// - Left + Right motors same speed = Forward/Backward
        /// - Left - Right motors difference = Yaw (Turn)
        /// - Front + Back vertical motors = Vertical movement (Up/Down)
        /// - Front - Back vertical motors difference = Pitch (Nose up/down)
        /// </summary>
        public void MotorToMovementInput()
        {
            // Forward: Average of left and right motors
            // Example: Left=1, Right=1 ? Forward=1 (full forward)
            //          Left=-1, Right=-1 ? Forward=-1 (full backward)
            //          Left=1, Right=-1 ? Forward=0 (turning in place)
            m_computedForward = (m_leftMotorPercent11 + m_rightMotorPercent11) / 2f;

            // Yaw: Half the difference between left and right motors
            // Example: Left=1, Right=-1 ? Yaw=1 (turn right)
            //          Left=-1, Right=1 ? Yaw=-1 (turn left)
            //          Left=1, Right=1 ? Yaw=0 (going straight)
            m_computedYaw = (m_leftMotorPercent11 - m_rightMotorPercent11) / 2f;

            // Vertical: Average of front and back vertical motors
            // Example: Front=1, Back=1 ? Vertical=1 (go up)
            //          Front=-1, Back=-1 ? Vertical=-1 (go down)
            //          Front=1, Back=-1 ? Vertical=0 (pitching only)
            m_computedVertical = (m_frontMotorPercent11 + m_backMotorPercent11) / 2f;

            // Pitch: Half the difference between front and back vertical motors
            // Example: Front=1, Back=-1 ? Pitch=1 (nose up)
            //          Front=-1, Back=1 ? Pitch=-1 (nose down)
            //          Front=1, Back=1 ? Pitch=0 (going straight up)
            m_computedPitch = (m_frontMotorPercent11 - m_backMotorPercent11) / 2f;
        }

        private void UpdateMotorVisuals()
        {
            // Spin propellers based on motor power
            if (m_leftMotor != null)
            {
                float spinAmount = m_leftMotorPercent11 * m_propellerSpinSpeed * Time.deltaTime;
                m_leftMotor.Rotate(Vector3.right, spinAmount, Space.Self);
            }
            if (m_rightMotor != null)
            {
                float spinAmount = m_rightMotorPercent11 * m_propellerSpinSpeed * Time.deltaTime;
                m_rightMotor.Rotate(Vector3.right, spinAmount, Space.Self);
            }
            if (m_backTopMotor != null)
            {
                float spinAmount = m_backMotorPercent11 * m_propellerSpinSpeed * Time.deltaTime;
                m_backTopMotor.Rotate(Vector3.forward, spinAmount, Space.Self);
            }
            if (m_frontTopMotor != null)
            {
                float spinAmount = m_frontMotorPercent11 * m_propellerSpinSpeed * Time.deltaTime;
                m_frontTopMotor.Rotate(Vector3.forward, spinAmount, Space.Self);
            }

            // Tilt motor pivots to show thrust direction
            if (m_leftRightMotorPivot != null)
            {
                float tilt = (m_leftMotorPercent11 - m_rightMotorPercent11) * 15f;
                m_leftRightMotorPivot.localRotation = Quaternion.Euler(tilt, 0f, 0f);
            }
            if (m_backFrontMotorPivot != null)
            {
                float tilt = (m_frontMotorPercent11 - m_backMotorPercent11) * 15f;
                m_backFrontMotorPivot.localRotation = Quaternion.Euler(0f, 0f, tilt);
            }
        }

        #region Helper Methods for Input Control

        /// <summary>Set both back motors for forward/backward movement</summary>
        public void SetForwardMotors(float percent)
        {
            m_leftMotorPercent11 = percent;
            m_rightMotorPercent11 = percent;
        }

        /// <summary>Set differential thrust for turning</summary>
        public void SetTurnMotors(float percent)
        {
            // Positive = turn right, Negative = turn left
            m_leftMotorPercent11 = percent;
            m_rightMotorPercent11 = -percent;
        }

        /// <summary>Set vertical motors for pitch control</summary>
        public void SetPitchMotors(float percent)
        {
            // Positive = nose up, Negative = nose down
            m_frontMotorPercent11 = percent;
            m_backMotorPercent11 = -percent;
        }

        /// <summary>Set vertical motors for up/down movement</summary>
        public void SetVerticalMotors(float percent)
        {
            // Positive = up, Negative = down
            m_frontMotorPercent11 = percent;
            m_backMotorPercent11 = percent;
        }

        /// <summary>Stop all motors</summary>
        public void StopAllMotors()
        {
            m_leftMotorPercent11 = 0f;
            m_rightMotorPercent11 = 0f;
            m_backMotorPercent11 = 0f;
            m_frontMotorPercent11 = 0f;
        }

        /// <summary>Combine forward + turn for back motors</summary>
        public void SetBackMotors(float forward, float turn)
        {
            m_leftMotorPercent11 = Mathf.Clamp(forward + turn, -1f, 1f);
            m_rightMotorPercent11 = Mathf.Clamp(forward - turn, -1f, 1f);
        }

        /// <summary>Combine vertical + pitch for vertical motors</summary>
        public void SetVerticalMotorsCombined(float vertical, float pitch)
        {
            m_frontMotorPercent11 = Mathf.Clamp(vertical + pitch, -1f, 1f);
            m_backMotorPercent11 = Mathf.Clamp(vertical - pitch, -1f, 1f);
        }

        #endregion

        #region Keyboard Input Example

        public void UpdateKeyboardInput()
        {
            float forward = 0f;
            float turn = 0f;
            float vertical = 0f;
            float pitch = 0f;

            // Forward/Backward
            if (Input.GetKey(KeyCode.W)) forward = 1f;
            if (Input.GetKey(KeyCode.S)) forward = -1f;

            // Turn Left/Right
            if (Input.GetKey(KeyCode.A)) turn = -1f;
            if (Input.GetKey(KeyCode.D)) turn = 1f;

            // Up/Down
            if (Input.GetKey(KeyCode.Space)) vertical = 1f;
            if (Input.GetKey(KeyCode.LeftShift)) vertical = -1f;

            // Pitch Up/Down
            if (Input.GetKey(KeyCode.UpArrow)) pitch = 1f;
            if (Input.GetKey(KeyCode.DownArrow)) pitch = -1f;

            SetBackMotors(forward, turn);
            SetVerticalMotorsCombined(vertical, pitch);
        }

        #endregion
    }

}