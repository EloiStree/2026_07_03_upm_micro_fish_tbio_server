using UnityEngine;

public class MicroFishMono_SetMeshInstanceColor : MonoBehaviour
{
    public Color m_color= Color.white;
    public Color m_targetColor= Color.black * 0.8f;
    public MeshRenderer m_meshRenderer;
    public bool m_useRandomColorAtAwake = false;

    public float m_timeLerpMultiplicator = 4f;

    [Header("Debug")]
    public Material m_dupliactedMaterial;


    public void Awake()
    {
        if (m_useRandomColorAtAwake)
        {
            Color color = new Color(Random.value, Random.value, Random.value);
            SetTargetColor(color);
        }
    }

    public void Update()
    {
        if (m_color != m_targetColor)
        {
            m_color = Color.Lerp(m_color, m_targetColor, Time.deltaTime * m_timeLerpMultiplicator);
            if (m_meshRenderer != null)
            {
                if (m_dupliactedMaterial == null)
                {
                    m_dupliactedMaterial = new Material(m_meshRenderer.material);
                    m_meshRenderer.material = m_dupliactedMaterial;
                }
                m_dupliactedMaterial.color = m_color;
            }
        }
    }

    public void SetTargetColor(Color color)
    {
        m_targetColor = color;
    }
}
