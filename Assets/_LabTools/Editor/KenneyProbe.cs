using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Diagnostic: logs the size / pivot / materials of Kenney models so scene
// layout can be done with real numbers.
public static class KenneyProbe
{
    const string Root = "Assets/Labs/Models/Kenney";

    public static void Probe()
    {
        var guids = AssetDatabase.FindAssets("t:Model", new[] { Root });
        System.Array.Sort(guids, (a, b) =>
            string.Compare(AssetDatabase.GUIDToAssetPath(a), AssetDatabase.GUIDToAssetPath(b)));
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
                continue;
            var go = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            var rs = go.GetComponentsInChildren<Renderer>();
            var b = new Bounds();
            var first = true;
            var mats = new HashSet<string>();
            var tris = 0;
            foreach (var r in rs)
            {
                if (first) { b = r.bounds; first = false; } else b.Encapsulate(r.bounds);
                foreach (var m in r.sharedMaterials) mats.Add(m ? m.name : "null");
                var mf = r.GetComponent<MeshFilter>();
                if (mf && mf.sharedMesh) tris += mf.sharedMesh.triangles.Length / 3;
            }
            Debug.Log($"[PROBE] {Path.GetFileNameWithoutExtension(path)}|size=({b.size.x:F2},{b.size.y:F2},{b.size.z:F2})|min=({b.min.x:F2},{b.min.y:F2},{b.min.z:F2})|tris={tris}|mats={string.Join(",", mats)}");
            Object.DestroyImmediate(go);
        }
        Debug.Log("[PROBE] done");
    }
}
