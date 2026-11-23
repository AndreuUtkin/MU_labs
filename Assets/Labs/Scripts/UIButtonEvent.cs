using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonEvent : MonoBehaviour
{
    public enum ButtonAction
    {
        None,
        ChangeColor,
        RandomColor,
        LoadScene,
        Quit,
        TogglePanel
    }

    public ButtonAction action = ButtonAction.None;
    public Renderer colorTarget;
    public Color color = Color.green;
    public string sceneName;
    public GameObject panel;

    Button m_Button;

    void Awake()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(OnClick);
    }

    void OnDestroy()
    {
        if (m_Button != null)
            m_Button.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        switch (action)
        {
            case ButtonAction.ChangeColor:
                if (colorTarget != null)
                    colorTarget.material.color = color;
                break;

            case ButtonAction.RandomColor:
                if (colorTarget != null)
                    colorTarget.material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.6f, 1f);
                break;

            case ButtonAction.LoadScene:
                if (!string.IsNullOrEmpty(sceneName))
                    SceneManager.LoadScene(sceneName);
                break;

            case ButtonAction.Quit:
                QuitApplication();
                break;

            case ButtonAction.TogglePanel:
                if (panel != null)
                    panel.SetActive(!panel.activeSelf);
                break;
        }
    }

    static void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
