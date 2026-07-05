using UnityEngine;

public class MicroFishMono_FocusTargetBall : MonoBehaviour
{
    public Camera m_camera;
    public Transform m_whatToRotate;
    public Transform m_whatToLookAt;

    [Header("Offsets")]
    public float m_heightOffsetY = 0.2f;
    public float m_distanceBehind = 0.2f;

    [Header("Smoothing")]
    public float m_positionLerp = 5f;
    public float m_rotationLerp = 8f;

    void Update()
    {
        // Desired position: behind the ball and slightly above it
        Vector3 targetPosition =
            m_whatToLookAt.position
            - m_whatToLookAt.forward * m_distanceBehind
            + Vector3.up * m_heightOffsetY;

        // Smoothly move
        m_whatToRotate.position = Vector3.Lerp(
            m_whatToRotate.position,
            targetPosition,
            Time.deltaTime * m_positionLerp);

        // Look at the ball
        Vector3 direction = m_whatToLookAt.position - m_whatToRotate.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            m_whatToRotate.rotation = Quaternion.Slerp(
                m_whatToRotate.rotation,
                targetRotation,
                Time.deltaTime * m_rotationLerp);
        }
    }
}