using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIVolumeSlider : MonoBehaviour
{
    Slider m_Slider;

    void Awake()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.onValueChanged.AddListener(OnChanged);
        OnChanged(m_Slider.value);
    }

    void OnDestroy()
    {
        if (m_Slider != null)
            m_Slider.onValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
    }
}
