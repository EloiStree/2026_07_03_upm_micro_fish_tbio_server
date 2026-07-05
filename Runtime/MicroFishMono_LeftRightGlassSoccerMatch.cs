using System;
using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_LeftRightGlassSoccerMatch : MonoBehaviour
{

    public UnityEvent<int,int> m_onScoreLeftRightChanged;
    public int m_scoreLeft = 0;
    public int m_scoreRight = 0;

    [SerializeField] private Transform m_rootCenterAnchor;
    [SerializeField] private Transform m_leftAnchorPoint;
    [SerializeField] private Transform m_rightAnchorPoint;
    [SerializeField] private Transform m_spawnPoint;
    [SerializeField] private Transform m_trackedBall;

    public float m_randomOffsetAtSpawn = 0.1f;

    public float m_blockScoreForSeconds = 1f;
    public float m_countdownBlockScore = 0f;

    [ContextMenu("Reset Match To Zero")]
    public void ResetMatchToZero()
    {
        m_scoreLeft = 0;
        m_scoreRight = 0;
        m_onScoreLeftRightChanged?.Invoke(m_scoreLeft, m_scoreRight);
    }

    public void ManualRemovePointToLeftTeam()
    {
        m_scoreLeft--;
        m_onScoreLeftRightChanged?.Invoke(m_scoreLeft, m_scoreRight);
    }
    public void ManualRemovePointToRightTeam()
    {
        m_scoreRight--;
        m_onScoreLeftRightChanged?.Invoke(m_scoreLeft, m_scoreRight);
    }

    public void ManualAddPointToLeftTeam()
    {
        m_scoreLeft++;
        m_onScoreLeftRightChanged?.Invoke(m_scoreLeft, m_scoreRight);
    }
    public void ManualAddPointToRightTeam()
    {
        m_scoreRight++;
        m_onScoreLeftRightChanged?.Invoke(m_scoreLeft, m_scoreRight);
    }


    public void Update()
    {
        if (m_blockScoreForSeconds > 0f)
        {
            m_blockScoreForSeconds -= Time.deltaTime;
            if (m_blockScoreForSeconds < 0f)
                m_blockScoreForSeconds = 0f;
            return;
        }
        GetWorldToLocal_DirectionalPoint(m_trackedBall.position, m_trackedBall.rotation, m_rootCenterAnchor.position, m_rootCenterAnchor.rotation,
            out Vector3 localBallPosition, out Quaternion localRotation);
        GetWorldToLocal_DirectionalPoint(m_leftAnchorPoint.position, m_leftAnchorPoint.rotation, m_rootCenterAnchor.position, m_rootCenterAnchor.rotation,
                    out Vector3 leftSide, out _);
        GetWorldToLocal_DirectionalPoint(m_rightAnchorPoint.position, m_rightAnchorPoint.rotation, m_rootCenterAnchor.position, m_rootCenterAnchor.rotation,
                    out Vector3 rightSide, out _);
        if (localBallPosition.x < leftSide.x)
        {
            ManualAddPointToRightTeam();
            RespawnBall();
            m_countdownBlockScore = m_blockScoreForSeconds;
        }
        else if (localBallPosition.x > rightSide.x)
        {
            ManualAddPointToLeftTeam();
            RespawnBall();
            m_countdownBlockScore = m_blockScoreForSeconds;
        }
    }

    private void RespawnBall()
    {
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-m_randomOffsetAtSpawn, m_randomOffsetAtSpawn), UnityEngine.Random.Range(-m_randomOffsetAtSpawn, m_randomOffsetAtSpawn), UnityEngine.Random.Range(-m_randomOffsetAtSpawn, m_randomOffsetAtSpawn));
        m_trackedBall.position = m_spawnPoint.position + randomOffset;
        m_trackedBall.rotation = m_spawnPoint.rotation;
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
