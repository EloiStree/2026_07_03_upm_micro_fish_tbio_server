using System;
using UnityEngine;
using UnityEngine.Events;


namespace Eloi.MicroFish
{ 

    public class MicroFishMono_MotorInput : MonoBehaviour
{

        private void OnValidate()
        {
            PushToUnityEvent();
        }

        [Range(-1f, 1f)]
        public float m_leftMotorPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_rightMotorPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_backMotorPercent11 = 0f;
        [Range(-1f, 1f)]
        public float m_frontMotorPercent11 = 0f;


        public UnityEvent<float> m_onMotorLeftPercent11;
        public UnityEvent<float> m_onMotorRightPercent11;
        public UnityEvent<float> m_onMotorBackPercent11;
        public UnityEvent<float> m_onMotorFrontPercent11;
        public UnityEvent<float, float, float, float> m_onMotorsInputUpdatedLRBF;

        public void PushToUnityEvent()
        {
            m_onMotorLeftPercent11?.Invoke(m_leftMotorPercent11);
            m_onMotorRightPercent11?.Invoke(m_rightMotorPercent11);
            m_onMotorBackPercent11?.Invoke(m_backMotorPercent11);
            m_onMotorFrontPercent11?.Invoke(m_frontMotorPercent11);
            m_onMotorsInputUpdatedLRBF?.Invoke(m_leftMotorPercent11, m_rightMotorPercent11, m_backMotorPercent11, m_frontMotorPercent11);
        }
        public void SetFourMotorWithFloat(float left, float right, float back, float front)
        {
            m_leftMotorPercent11 = Mathf.Clamp(left, -1f, 1f);
            m_rightMotorPercent11 = Mathf.Clamp(right, -1f, 1f);
            m_backMotorPercent11 = Mathf.Clamp(back, -1f, 1f);
            m_frontMotorPercent11 = Mathf.Clamp(front, -1f, 1f);
            PushToUnityEvent();
        }

        public void SetMotorsWithFloatArray(params float[] motors)
        { 
            if (motors.Length >= 1) m_leftMotorPercent11 = Mathf.Clamp(motors[0], -1f, 1f);
            if (motors.Length >= 2) m_rightMotorPercent11 = Mathf.Clamp(motors[1], -1f, 1f);
            if (motors.Length >= 3) m_backMotorPercent11 = Mathf.Clamp(motors[2], -1f, 1f);
            if (motors.Length >= 4) m_frontMotorPercent11 = Mathf.Clamp(motors[3], -1f, 1f);
            PushToUnityEvent();
        }

        public void SetMotorHorizontalLeft() => SetHorizontalMotorFloat(-1f, 1f);
        public void SetMotorHorizontalRight() => SetHorizontalMotorFloat(1f, -1f);
        public void SetMotorHorizontalStop() => SetHorizontalMotorFloat(0f, 0f);

        public void SetMotorVerticalUp() => SetVerticalMotorFloat(1f, 1f);
        public void SetMotorVerticalDown() => SetVerticalMotorFloat(-1, -1f);
        public void SetMotorVerticalStop() => SetVerticalMotorFloat(0f, 0f);

        public void SetMotorHorizontalForward() => SetHorizontalMotorFloat(1f, 1f);
        public void SetMotorHorizontalBackward() => SetHorizontalMotorFloat(-1f, -1f);




        public void  SetHorizontalMotorFloat(float left, float right)
        {
            m_leftMotorPercent11 = Mathf.Clamp(left, -1f, 1f);
            m_rightMotorPercent11 = Mathf.Clamp(right, -1f, 1f);
            PushToUnityEvent();
        }

        public void SetVerticalMotorFloat(float back, float front)
        {
            m_backMotorPercent11 = Mathf.Clamp(back, -1f, 1f);
            m_frontMotorPercent11 = Mathf.Clamp(front, -1f, 1f);
            PushToUnityEvent();
        }

        public void SetMotorLeftPercent11(float percent)
        {
            m_leftMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
            PushToUnityEvent();
        }
        public void SetMotorRightPercent11(float percent)
        {
            m_rightMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
            PushToUnityEvent();
        }

        public void SetMotorBackPercent11(float percent)
        {
            m_backMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
            PushToUnityEvent();
        }
        public void SetMotorFrontPercent11(float percent)
        {
            m_frontMotorPercent11 = Mathf.Clamp(percent, -1f, 1f);
            PushToUnityEvent();
        }

        public float GetMotorLeftPercent11() => m_leftMotorPercent11;
        public float GetMotorRightPercent11() => m_rightMotorPercent11;
        public float GetMotorBackPercent11() => m_backMotorPercent11;
        public float GetMotorFrontPercent11() => m_frontMotorPercent11;

        public void GetMotorsPercent11AsLRBF(out float left, out float right, out float back, out float front)
        {
            left = m_leftMotorPercent11;
            right = m_rightMotorPercent11;
            back = m_backMotorPercent11;
            front = m_frontMotorPercent11;
        }
    }
}