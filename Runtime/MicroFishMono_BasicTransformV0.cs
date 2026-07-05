using UnityEngine;


namespace Eloi.MicroFish
{

    /// <summary>
    ///  Submarine composed of two back motor lef and right.
    ///  And two vertical motor back and front.
    /// </summary>
    public class MicroFishMono_BasicTransformV0: MonoBehaviour
    {
        public Transform m_whatToMove;
        [Header("Motor Position")]
        public Transform m_leftMotor;
        public Transform m_rightMotor;
        public Transform m_backTopMotor;
        public Transform m_frontTopMotor;

        [Header("Motor Pivot")]
        public Transform m_backFrontMotorPivot;
        public Transform m_leftRightMotorPivot;
        public Transform m_directionToMoveForwardAndUp;

        public bool m_hasBattery = false;

        [Header("Motor Percentages")]
        [Range(-1f, 1f)]
        public float m_leftMotorPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_rightMotorPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_backMotorPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_frontMotorPercent11 = 0f;
        [Header("Motor Percentages")]
        [Range(-1f, 1f)]
        public float m_leftMotorPercent11Lerp = 0f;
        [Range(-1f, 1f)]
        public float m_rightMotorPercent11Lerp = 0f;
        [Range(-1f, 1f)]
        public float m_backMotorPercent11Lerp = 0f;
        [Range(-1f, 1f)]
        public float m_frontMotorPercent11Lerp = 0f;


        [Header("Movement")]
        [Range(-1f, 1f)]
        public float m_moveForwardPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_moveDownUpPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_rotateLeftRightPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_boilingPercent11 = 0f;


        [Header("Values")]
        public float m_leftRightRotationAroundVerticalValue = 0f;
        public float m_leftRightRotationAroundVerticalAngleMax = 90f;
        public float m_boilingRotationAngleMax = 30f;
        public float m_moveBackFrontSpeedMax = 1;
        public float m_moveDownUpSpeedMax = 0.2f;

        public float m_lerpFactor = 5f;



        public void SetBatteryState(bool hasBattery)
        {
            m_hasBattery = hasBattery;

        }

        public void FixedUpdate()
        {

            

            float deltaTime = Time.fixedDeltaTime;

            m_rightMotorPercent11Lerp = Mathf.Lerp(m_rightMotorPercent11Lerp, m_rightMotorPercent11, deltaTime * m_lerpFactor);
            m_leftMotorPercent11Lerp = Mathf.Lerp(m_leftMotorPercent11Lerp, m_leftMotorPercent11, deltaTime * m_lerpFactor);
            m_backMotorPercent11Lerp = Mathf.Lerp(m_backMotorPercent11Lerp, m_backMotorPercent11, deltaTime * m_lerpFactor);
            m_frontMotorPercent11Lerp = Mathf.Lerp(m_frontMotorPercent11Lerp, m_frontMotorPercent11, deltaTime * m_lerpFactor);

            MotorToMovementInput();
            if (!m_hasBattery)
                return;
            m_leftRightRotationAroundVerticalValue += m_rotateLeftRightPercent11 * m_leftRightRotationAroundVerticalAngleMax * deltaTime;
            m_whatToMove.rotation = Quaternion.Euler(0,- m_leftRightRotationAroundVerticalValue, 0f);
            m_whatToMove.Rotate(Vector3.right, m_boilingPercent11 * m_boilingRotationAngleMax, Space.Self);
            m_whatToMove.Translate(m_directionToMoveForwardAndUp.forward * m_moveForwardPercent11 * m_moveBackFrontSpeedMax*deltaTime, Space.World);
            m_whatToMove.Translate(m_directionToMoveForwardAndUp.up * m_moveDownUpPercent11 * m_moveDownUpSpeedMax * deltaTime, Space.World);

        }

        public void MotorToMovementInput()
        {
            // === Left/Right Motor Differential Drive ===
            // Both same direction → Forward/Backward
            // Opposite directions → Yaw rotation (like a tank)
            float leftRightForward = (m_leftMotorPercent11Lerp + m_rightMotorPercent11Lerp) * 0.5f;
            float leftRightYaw = (m_rightMotorPercent11Lerp - m_leftMotorPercent11Lerp) * 0.5f;

            // === Back/Front Motor Differential Drive ===
            // Both same direction → Forward/Backward (contributes to thrust)
            // Opposite directions → Pitch/Boiling (nose up/down)
            float backFrontForward = (m_backMotorPercent11Lerp + m_frontMotorPercent11Lerp) * 0.5f;
            float backFrontPitch = (m_backMotorPercent11Lerp - m_frontMotorPercent11Lerp) * 0.5f;

            // === Combine Outputs ===
            m_moveForwardPercent11 = leftRightForward ;
            m_rotateLeftRightPercent11 = leftRightYaw;
            m_boilingPercent11 = backFrontPitch;
            m_moveDownUpPercent11 = backFrontForward;
        }

        public void Reset()
        {
            m_whatToMove = transform;
        }

        public void SetMotorLeft(float percent)
        {
            m_leftMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
        }
        public void SetMotorRight(float percent)
        {
            m_rightMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
        }
        public void SetMotorBack(float percent)
        {
            m_backMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
        }
        public void SetMotorFront(float percent)
        {
            m_frontMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
        }

        public void SetMotorsInFormatLRBF(float left, float right, float back, float front)
        {
            m_leftMotorPercent11 = Mathf.Clamp(left, -1f, 1f);
            m_rightMotorPercent11 = Mathf.Clamp(right, -1f, 1f);
            m_backMotorPercent11 = Mathf.Clamp(back, -1f, 1f);
            m_frontMotorPercent11 = Mathf.Clamp(front, -1f, 1f);
        }



    }

}