using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Renders simple screenshots of every scene so the result can be reviewed
// outside the Unity editor (and by tooling that can read PNG files).
public static class SceneScreenshot
{
    const string OutDir = "Assets/Screenshots";

    [MenuItem("Tools/Capture Scene Screenshots")]
    public static void CaptureAll()
    {
        Directory.CreateDirectory(OutDir);
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (!s.enabled || string.IsNullOrEmpty(s.path))
                continue;
            var scene = EditorSceneManager.OpenScene(s.path, OpenSceneMode.Single);
            var name = Path.GetFileNameWithoutExtension(s.path);
            Capture(name, new Vector3(0f, 1.7f, -2.4f), Quaternion.Euler(4f, 0f, 0f), "front");
            Capture(name, new Vector3(-2.6f, 2.4f, -2.6f), Quaternion.Euler(20f, 40f, 0f), "iso");
        }
        AssetDatabase.Refresh();
        Debug.Log("[SceneScreenshot] Done -> " + OutDir);
    }

    static void Capture(string sceneName, Vector3 pos, Quaternion rot, string tag)
    {
        var go = new GameObject("~ShotCam");
        var cam = go.AddComponent<Camera>();
        cam.transform.position = pos;
        cam.transform.rotation = rot;
        cam.fieldOfView = 62f;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 200f;
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.enabled = true;

        var rt = new RenderTexture(1280, 720, 24);
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        var path = $"{OutDir}/{sceneName}_{tag}.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());

        cam.targetTexture = null;
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(tex);
        Object.DestroyImmediate(go);
    }
}
