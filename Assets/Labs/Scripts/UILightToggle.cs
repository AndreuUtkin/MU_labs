using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UILightToggle : MonoBehaviour
{
    public Light targetLight;
    public Renderer indicator;

    Toggle m_Toggle;

    void Awake()
    {
        m_Toggle = GetComponent<Toggle>();
        m_Toggle.onValueChanged.AddListener(OnChanged);
        OnChanged(m_Toggle.isOn);
    }

    void OnDestroy()
    {
        if (m_Toggle != null)
            m_Toggle.onValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(bool on)
    {
        if (targetLight != null)
            targetLight.enabled = on;

        if (indicator != null)
            indicator.material.color = on ? new Color(0.2f, 0.9f, 0.3f) : new Color(0.3f, 0.1f, 0.1f);
    }
}
