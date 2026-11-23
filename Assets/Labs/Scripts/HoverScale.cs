using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class HoverScale : MonoBehaviour
{
    public float hoverScale = 1.25f;
    public float transitionSpeed = 8f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Vector3 m_OriginalScale;
    bool m_Hovered;
    float m_T;

    void Awake()
    {
        m_OriginalScale = transform.localScale;

        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    void OnHoverEnter(HoverEnterEventArgs args) => m_Hovered = true;

    void OnHoverExit(HoverExitEventArgs args) => m_Hovered = false;

    void Update()
    {
        float target = m_Hovered ? 1f : 0f;
        m_T = Mathf.MoveTowards(m_T, target, Time.deltaTime * transitionSpeed);
        float curve = scaleCurve.Evaluate(m_T);
        transform.localScale = Vector3.Lerp(m_OriginalScale, m_OriginalScale * hoverScale, curve);
    }
}
