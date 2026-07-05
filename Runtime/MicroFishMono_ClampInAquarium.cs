using UnityEngine;

public class MicroFishMono_ClampInAquarium : MonoBehaviour
{


    public Transform m_startDownPoint;
    public Transform m_endUpPoint;

    public Transform[] m_pointsToClamp;


    public void Update()
    {
       GetWorldToLocal_DirectionalPoint(m_endUpPoint.position, m_endUpPoint.rotation, m_startDownPoint.position, m_startDownPoint.rotation, out Vector3 dimension, out _);

        foreach (Transform t in m_pointsToClamp) { 
        
            GetWorldToLocal_DirectionalPoint(t.position, t.rotation, m_startDownPoint.position, m_startDownPoint.rotation, out Vector3 localPosition, out Quaternion localRotation);
            if (localPosition.x < 0f || localPosition.x > dimension.x || localPosition.y < 0f || localPosition.y > dimension.y || localPosition.z < 0f || localPosition.z > dimension.z)
            {
                localPosition.x = Mathf.Clamp(localPosition.x, 0f, dimension.x);
                localPosition.y = Mathf.Clamp(localPosition.y, 0f, dimension.y);
                localPosition.z = Mathf.Clamp(localPosition.z, 0f, dimension.z);

                if (localPosition.x>dimension.x) localPosition.x = dimension.x;
                if (localPosition.y>dimension.y) localPosition.y = dimension.y;
                if (localPosition.z>dimension.z) localPosition.z = dimension.z;
                if (localPosition.x<0f) localPosition.x = 0f;
                if (localPosition.y<0f) localPosition.y = 0f;
                if (localPosition.z<0f) localPosition.z = 0f;

                GetLocalToWorld_DirectionalPoint(localPosition, localRotation, m_startDownPoint.position, m_startDownPoint.rotation, out Vector3 worldPosition, out Quaternion worldRotation);
                t.position = worldPosition;
                t.rotation = worldRotation;
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
