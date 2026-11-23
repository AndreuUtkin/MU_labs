using System.Text;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneDump
{
    const string DemoScene = "Assets/Samples/XR Interaction Toolkit/3.7.0/Starter Assets/DemoScene.unity";
    const string OutFile = "/tmp/opencode/scene_dump.txt";

    [MenuItem("Tools/Dump DemoScene")]
    public static void Run()
    {
        var sb = new StringBuilder();
        DumpScene(DemoScene, sb);
        File.WriteAllText(OutFile, sb.ToString());
        Debug.Log("Dumped to " + OutFile);
    }

    [MenuItem("Tools/Dump Lab Scenes")]
    public static void DumpLabs()
    {
        var sb = new StringBuilder();
        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Labs/Scenes" });
        foreach (var guid in guids)
            DumpScene(AssetDatabase.GUIDToAssetPath(guid), sb);
        File.WriteAllText("/tmp/opencode/lab_scene_dump.txt", sb.ToString());
        Debug.Log("Lab scenes dumped.");
    }

    static void DumpScene(string path, StringBuilder sb)
    {
        var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        sb.AppendLine("########## " + path + " ##########");
        foreach (var root in scene.GetRootGameObjects())
        {
            sb.AppendLine("ROOT: " + root.name);
            DumpRec(root.transform, sb, 1);
        }
    }

    static void DumpRec(Transform t, StringBuilder sb, int depth)
    {
        var indent = new string(' ', depth * 2);
        sb.Append(indent).Append("- ").Append(t.name);
        var comps = t.GetComponents<Component>();
        sb.Append("  [");
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] == null) { sb.Append("MISSING"); }
            else sb.Append(comps[i].GetType().Name);
            if (i < comps.Length - 1) sb.Append(", ");
        }
        sb.AppendLine("]");
        for (int i = 0; i < t.childCount; i++)
            DumpRec(t.GetChild(i), sb, depth + 1);
    }
}
