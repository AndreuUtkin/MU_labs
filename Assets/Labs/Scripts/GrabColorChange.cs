using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabColorChange : MonoBehaviour
{
    public Color highlightColor = Color.green;
    public Color returnColor = Color.white;
    public float transitionSpeed = 10f;
    public bool useLerpReturn = true;

    XRGrabInteractable m_Grab;
    Material[] m_Materials;
    Color[] m_Original;
    Color[] m_TargetColors;

    void Awake()
    {
        m_Grab = GetComponent<XRGrabInteractable>();
        var renderers = GetComponentsInChildren<Renderer>();
        m_Materials = new Material[renderers.Length];
        m_Original = new Color[renderers.Length];
        m_TargetColors = new Color[renderers.Length];
        for (var i = 0; i < renderers.Length; i++)
        {
            m_Materials[i] = renderers[i].material;
            m_Original[i] = m_Materials[i].color;
            m_TargetColors[i] = m_Original[i];
        }

        m_Grab.selectEntered.AddListener(OnGrabbed);
        m_Grab.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        if (m_Grab == null)
            return;
        m_Grab.selectEntered.RemoveListener(OnGrabbed);
        m_Grab.selectExited.RemoveListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args) => SetHighlight();

    void OnReleased(SelectExitEventArgs args) => SetReturn();

    void SetHighlight()
    {
        for (var i = 0; i < m_Materials.Length; i++)
            m_TargetColors[i] = highlightColor;
    }

    void SetReturn()
    {
        for (var i = 0; i < m_Materials.Length; i++)
            m_TargetColors[i] = returnColor;
    }

    void Update()
    {
        if (useLerpReturn)
        {
            for (var i = 0; i < m_Materials.Length; i++)
            {
                m_Materials[i].color = Color.Lerp(m_Materials[i].color, m_TargetColors[i], Time.deltaTime * transitionSpeed);
            }
        }
        else
        {
            for (var i = 0; i < m_Materials.Length; i++)
            {
                m_Materials[i].color = m_TargetColors[i];
            }
        }
    }
}
