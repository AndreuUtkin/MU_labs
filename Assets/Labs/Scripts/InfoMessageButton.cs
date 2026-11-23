using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class InfoMessageButton : MonoBehaviour
{
    public GameObject messageObject;
    public bool toggle = true;
    public float autoHideDelay = 2f;

    float m_HideAt = -1f;

    void Awake()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnPressed);
        if (messageObject != null)
            messageObject.SetActive(false);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        if (messageObject == null)
            return;

        if (toggle)
        {
            messageObject.SetActive(!messageObject.activeSelf);
            m_HideAt = -1f;
        }
        else
        {
            messageObject.SetActive(true);
            m_HideAt = autoHideDelay > 0f ? Time.time + autoHideDelay : -1f;
        }
    }

    void Update()
    {
        if (m_HideAt > 0f && Time.time >= m_HideAt)
        {
            messageObject.SetActive(false);
            m_HideAt = -1f;
        }
    }
}
