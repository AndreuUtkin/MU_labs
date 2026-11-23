using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIColorSlider : MonoBehaviour
{
    public Renderer target;
    public float minHue;
    public float maxHue = 1f;
    public float saturation = 0.85f;
    public float brightness = 0.9f;

    Slider m_Slider;

    void Awake()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.onValueChanged.AddListener(OnChanged);
    }

    void OnDestroy()
    {
        if (m_Slider != null)
            m_Slider.onValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(float value)
    {
        if (target == null)
            return;

        var t = Mathf.InverseLerp(m_Slider.minValue, m_Slider.maxValue, value);
        target.material.color = Color.HSVToRGB(Mathf.Lerp(minHue, maxHue, t), saturation, brightness);
    }
}
