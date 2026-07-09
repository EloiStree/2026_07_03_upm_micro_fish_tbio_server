using UnityEngine;

public class MicroFishMono_ForceFromCollisionPoint : MonoBehaviour
{


    public Transform m_targetCenter;
    public float m_forceMagnitude = 10f; 
    public ForceMode m_forceMode = ForceMode.Impulse;
    public LayerMask m_collisionLayerMask = ~0; 


    private void Reset()
    {
        m_targetCenter= transform;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if ((m_collisionLayerMask.value & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }
        ContactPoint contact = collision.contacts[0];
        Vector3 forceDirection = (m_targetCenter.position - contact.point).normalized;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(forceDirection * m_forceMagnitude, m_forceMode);
        }
    }
}
