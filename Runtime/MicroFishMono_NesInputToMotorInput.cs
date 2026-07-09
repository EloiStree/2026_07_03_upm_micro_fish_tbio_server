using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_NesInputToMotorInput : MonoBehaviour
{

    public UnityEvent<float, float, float, float> m_onMotorsInputLRBF;


    public bool m_buttonArrowUp;
    public bool m_buttonArrowDown;
    public bool m_buttonArrowLeft;
    public bool m_buttonArrowRight;

    public bool m_buttonA;
    public bool m_buttonB;
    public bool m_buttonMenuLeft;
    public bool m_buttonMenuRight;


    public float m_motorInputLeft;
    public float m_motorInputRight;
    public float m_motorInputBack;
    public float m_motorInputFront;

    

    public void Update()
    {


        if (m_buttonArrowLeft || m_buttonArrowRight)
        {

            if (m_buttonArrowLeft && !m_buttonArrowRight)
            {
                m_motorInputLeft = 1.0f;
                m_motorInputRight = 0.0f;
            }
            else if (!m_buttonArrowLeft && m_buttonArrowRight)
            {
                m_motorInputLeft = 0.0f;
                m_motorInputRight = 1.0f;
            }
            else if (m_buttonArrowLeft && m_buttonArrowRight)
            {
                m_motorInputLeft = 1.0f;
                m_motorInputRight = 1.0f;
            }

        }
        else {
            if (m_buttonA && !m_buttonB) {

                m_motorInputLeft = 1.0f;
                m_motorInputRight = 1.0f;
            }
            else if (!m_buttonA && m_buttonB)
            {
                m_motorInputLeft = -1.0f;
                m_motorInputRight = -1.0f;
            }
        }

        if (m_buttonArrowUp || m_buttonArrowDown)
        {
            if (m_buttonArrowUp && !m_buttonArrowDown)
            {
                m_motorInputFront = 1f;
                m_motorInputBack = 0f;
            }
            else if (!m_buttonArrowUp && m_buttonArrowDown)
            {
                m_motorInputFront = -1f;
                m_motorInputBack = 0f;
            }
        }



    }

}
