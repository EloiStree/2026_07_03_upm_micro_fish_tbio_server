using UnityEngine;

public class MicroFishMono_SetMeshInstanceTexture : MonoBehaviour
{

    public Texture m_texture;
    public MeshRenderer m_meshRenderer;
    [Header("Debug")]
    public Material m_dupliactedMaterial;


    public void Awake()
    {
        SetTexture(m_texture);
    }

    public void SetTexture(Texture texture)
    {
     
        if (m_meshRenderer != null)
        {
            if (m_dupliactedMaterial == null)
            {
                m_dupliactedMaterial = new Material(m_meshRenderer.material);
                m_meshRenderer.material = m_dupliactedMaterial;
            }
            m_dupliactedMaterial.mainTexture = texture;
        }
    }
}
