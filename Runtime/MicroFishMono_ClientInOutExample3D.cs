using Eloi.MicroFish;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_ClientInOutExample3D : MonoBehaviour
{
    
    //public int m_playerCount = 0;
    //public Vector3 m_dimension = Vector3.zero;
    //public float m_gameTimeInSeconds = 0f;
    //public int m_scoreLeft;
    //public int m_scoreRight;
    //public Vector3 m_ballPosition = Vector3.zero;
    //public float m_ballRadius = 0.2f;
    //public Vector3[] m_playerPosition;
    //public Quaternion[] m_playerRotation;
    //public Vector3[] m_playerEuler;
    //public int[] m_playerClaimedInteger;
    //public string [] m_playerPublicKey;
    //public float[] m_playerBatteryLevel;


    public bool m_useFixedUpdateToRefresh = true;
    public float m_tablePercent = 1f;
    public Transform m_tableBackLeft;
    public Transform m_tableFrontRight;
    public Transform m_sizeTransform;
    public Transform m_ballTransform;
    public Transform m_aquariumSizeTransform;

    public MicroFishMono_ClientInOutExample m_source;


    public MicroFishMono_SetMeshInstanceColor[] m_playersColor;
    public Transform [] m_playersLocalPosition;

    public MicroFishMono_MotorInput[] m_playersMotorsInput; 

    public void FixedUpdate()
    {
        if (m_useFixedUpdateToRefresh)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        GetWorldToLocal_DirectionalPoint(m_tableFrontRight.position, m_tableFrontRight.rotation, m_tableBackLeft.position, m_tableBackLeft.rotation,
            out Vector3 local, out _);


        Vector3 dimension = m_source.m_dimension;
        float leftRightSize = m_tablePercent * local.x/ m_source.m_dimension.x;
        Vector3 localHalfOffset= new Vector3(-dimension.x * 0.5f,0, -dimension.z * 0.5f);
        m_sizeTransform.localScale = Vector3.one * leftRightSize;
        m_aquariumSizeTransform.localScale = new Vector3(dimension.x, dimension.y , dimension.z);
        m_aquariumSizeTransform.localPosition = new Vector3(0, dimension.y*0.5f,0);
        m_ballTransform.localPosition = m_source.m_ballPosition + localHalfOffset;
        m_ballTransform.localScale = Vector3.one * m_source.m_ballRadius*2f ;

       


        for (int i = 0; i < m_source.m_playerCount; i++)
        {
            if (i < m_playersLocalPosition.Length)
            {
                m_playersLocalPosition[i].localPosition = m_source.m_playerPosition[i] + localHalfOffset;
                m_playersLocalPosition[i].localRotation = m_source.m_playerRotation[i];
                if (i < m_playersColor.Length)
                {
                    m_playersColor[i].SetTargetColor(m_source.m_playersColor[i]);
                }

                if (i < m_playersMotorsInput.Length)
                { 
                    var inputs = m_source.m_playersMotors[i];
                    m_playersMotorsInput[i].SetFourMotorWithFloat(
                        inputs.m_leftMotorPercent11,
                        inputs.m_rightMotorPercent11,
                        inputs.m_backMotorPercent11,
                        inputs.m_frontMotorPercent11
                    
                    );
                }
            }


        }



    }
    public static void GetWorldToLocal_DirectionalPoint(in Vector3 worldPosition, in Quaternion worldRotation, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 localPosition, out Quaternion localRotation)
    {
        localRotation = Quaternion.Inverse(rotationReference) * worldRotation;
        localPosition = Quaternion.Inverse(rotationReference) * (worldPosition - positionReference);
    }
    public static void GetLocalToWorld_DirectionalPoint(in Vector3 localPosition, in Quaternion localRotation, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 worldPosition, out Quaternion worldRotation)
    {
        /// I need to verify the commutativity of this code. 
        /// I think it was ok then had a bug in a game link to this methode and thr commutative property
        worldRotation = rotationReference * localRotation;
        worldPosition = (rotationReference * localPosition) + (positionReference);
    }
}
