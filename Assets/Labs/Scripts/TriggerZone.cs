using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public IndicatorLight indicator;
    public Renderer visual;
    public Color enterColor = new Color(0.2f, 0.6f, 1f);
    public float colorLerpSpeed = 6f;

    Material m_VisualMaterial;
    Color m_BaseColor;
    bool m_InZone;

    void Awake()
    {
        if (visual == null)
            visual = GetComponent<Renderer>();
        if (visual != null)
        {
            m_VisualMaterial = visual.material;
            m_BaseColor = m_VisualMaterial.color;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        m_InZone = true;
        if (indicator != null)
            indicator.SetOn(true);
    }

    void OnTriggerExit(Collider other)
    {
        m_InZone = false;
        if (indicator != null)
            indicator.SetOn(false);
    }

    void Update()
    {
        if (m_VisualMaterial == null)
            return;
        var target = m_InZone ? enterColor : m_BaseColor;
        m_VisualMaterial.color = Color.Lerp(m_VisualMaterial.color, target, Time.deltaTime * colorLerpSpeed);
    }
}
