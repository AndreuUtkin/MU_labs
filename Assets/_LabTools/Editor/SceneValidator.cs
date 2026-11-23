using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneValidator
{
    [MenuItem("Tools/Validate Lab Scenes")]
    public static void ValidateAll()
    {
        var sb = new StringBuilder();
        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Labs/Scenes" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            sb.AppendLine("=== " + path + " ===");
            var issues = 0;
            foreach (var root in scene.GetRootGameObjects())
                issues += CheckRecursive(root.transform, sb);
            sb.AppendLine(issues == 0 ? "  OK - no issues" : "  ISSUES: " + issues);
        }

        System.IO.File.WriteAllText("/tmp/opencode/lab_validation.txt", sb.ToString());
        Debug.Log("[SceneValidator]\n" + sb);
    }

    static int CheckRecursive(Transform t, StringBuilder sb)
    {
        var issues = 0;
        foreach (var c in t.GetComponents<Component>())
        {
            if (c == null)
            {
                sb.AppendLine("  MISSING SCRIPT on " + GetPath(t));
                issues++;
            }
        }

        var renderer = t.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial == null)
        {
            sb.AppendLine("  MISSING MATERIAL on " + GetPath(t));
            issues++;
        }

        for (var i = 0; i < t.childCount; i++)
            issues += CheckRecursive(t.GetChild(i), sb);

        return issues;
    }

    static string GetPath(Transform t)
    {
        var path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
