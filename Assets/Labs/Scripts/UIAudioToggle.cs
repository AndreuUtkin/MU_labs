using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIAudioToggle : MonoBehaviour
{
    public AudioSource target;

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
        if (target != null)
            target.mute = !on;
    }
}
