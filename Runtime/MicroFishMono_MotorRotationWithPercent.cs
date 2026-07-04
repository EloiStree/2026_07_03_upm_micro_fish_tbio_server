using UnityEngine;

public class MicroFishMono_MotorRotationWithPercent : MonoBehaviour
{

    public Transform m_whatToRotateLocal;

    public Vector3 m_rotationAxis=Vector3.forward;
    public float m_maxRotationSpeed = 360f;
    [Range(-1f, 1f)]
    public float m_motorPowerPercent = 0f;
    public bool m_inverseRotation = false;

    private void Reset()
    {
        m_whatToRotateLocal= transform;
    }
    void Update()
    {

        if (m_whatToRotateLocal != null)
        {
            float rotationSpeed = m_motorPowerPercent * m_maxRotationSpeed;
            if (m_inverseRotation)
            {
                rotationSpeed *= -1f;
            }
            m_whatToRotateLocal.Rotate(m_rotationAxis, rotationSpeed * Time.deltaTime,Space.Self);
        }
    }

    public void SetMotorPowerPercent11(float percent)
    {
        m_motorPowerPercent = Mathf.Clamp(percent, -1f, 1f);
    }

    public void SetMotorOn()
    {
        m_motorPowerPercent = 1f;
    }
    public void SetMotorOnInverse()
    {
        m_motorPowerPercent = -1f;
    }
    public void StopMotor()
    {
        m_motorPowerPercent = 0f;
    }   
}
