using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class InteractionSound : MonoBehaviour
{
    public AudioClip grabClip;
    public AudioClip releaseClip;
    public AudioClip hoverClip;
    public float pitchVariance = 0.1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    AudioSource m_Source;
    XRGrabInteractable m_Grab;

    void Awake()
    {
        m_Source = GetComponent<AudioSource>();
        if (m_Source == null)
            m_Source = gameObject.AddComponent<AudioSource>();
        m_Source.playOnAwake = false;
        m_Source.spatialBlend = 1f;

        m_Grab = GetComponent<XRGrabInteractable>();
        m_Grab.selectEntered.AddListener(OnGrabbed);
        m_Grab.selectExited.AddListener(OnReleased);
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
        }
    }

    void OnDestroy()
    {
        if (m_Grab == null)
            return;
        m_Grab.selectEntered.RemoveListener(OnGrabbed);
        m_Grab.selectExited.RemoveListener(OnReleased);
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args) => PlayRandom(grabClip);

    void OnReleased(SelectExitEventArgs args) => PlayRandom(releaseClip);

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        PlayRandom(hoverClip);
    }

    void PlayRandom(AudioClip clip)
    {
        if (clip == null)
            return;

        m_Source.pitch = Random.Range(minPitch, maxPitch);
        m_Source.PlayOneShot(clip);
    }
}
