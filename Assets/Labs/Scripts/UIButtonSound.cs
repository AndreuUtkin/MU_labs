using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;
    public float volume = 0.7f;

    Button m_Button;

    void Awake()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(Play);
    }

    void OnDestroy()
    {
        if (m_Button != null)
            m_Button.onClick.RemoveListener(Play);
    }

    void Play()
    {
        if (clip == null)
            return;

        if (source != null)
            source.PlayOneShot(clip, volume);
        else
            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : transform.position, volume);
    }
}
