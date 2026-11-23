using UnityEngine;

public class IndicatorLight : MonoBehaviour
{
    public Color offColor = new Color(0.25f, 0.05f, 0.05f);
    public Color onColor = new Color(0.1f, 0.9f, 0.2f);
    public bool initialState;
    public bool blinking;
    public float blinkRate = 2f;
    public bool autoReset;
    public float resetDelay = 2f;

    Material m_Material;
    bool m_On;
    bool m_BlinkPhase = true;
    float m_BlinkTimer;
    float m_ResetTimer;

    void Awake()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            m_Material = renderer.material;
        m_On = initialState;
        Apply();
    }

    void Apply()
    {
        if (m_Material == null)
            return;
        var color = m_On ? onColor : offColor;
        m_Material.color = color;
        m_Material.EnableKeyword("_EMISSION");
        m_Material.SetColor("_EmissionColor", color * (m_On ? 1.5f : 0.15f));
    }

    public void SetOn(bool on)
    {
        m_On = on;
        m_ResetTimer = 0f;
        m_BlinkTimer = 0f;
        m_BlinkPhase = true;
        Apply();
    }

    public void Toggle() => SetOn(!m_On);

    void Update()
    {
        if (blinking && m_On && m_Material != null)
        {
            m_BlinkTimer += Time.deltaTime;
            var period = 1f / Mathf.Max(0.01f, blinkRate);
            if (m_BlinkTimer >= period)
            {
                m_BlinkTimer -= period;
                m_BlinkPhase = !m_BlinkPhase;
                var color = m_BlinkPhase ? onColor : offColor;
                m_Material.color = color;
                m_Material.EnableKeyword("_EMISSION");
                m_Material.SetColor("_EmissionColor", color * (m_BlinkPhase ? 1.5f : 0.15f));
            }
        }

        if (autoReset && m_On)
        {
            m_ResetTimer += Time.deltaTime;
            if (m_ResetTimer >= resetDelay)
                SetOn(false);
        }
    }
}
