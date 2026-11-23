using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoOpenLabScene
{
    const string FirstScene = "Assets/Labs/Scenes/Lab1_Setup.unity";

    static AutoOpenLabScene()
    {
        EditorApplication.delayCall += OpenIfUntitledEmpty;
    }

    static void OpenIfUntitledEmpty()
    {
        var active = EditorSceneManager.GetActiveScene();
        var isEmptyDefault = string.IsNullOrEmpty(active.path) && !active.isDirty && active.rootCount <= 2;
        if (isEmptyDefault && File.Exists(FirstScene))
        {
            EditorSceneManager.OpenScene(FirstScene, OpenSceneMode.Single);
            Debug.Log("[AutoOpenLabScene] Opened " + FirstScene);
        }
    }
}
