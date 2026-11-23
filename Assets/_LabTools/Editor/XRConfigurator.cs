using System.IO;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

public static class XRConfigurator
{
    public static void Configure()
    {
        var buildTargetGroup = BuildTargetGroup.Standalone;

        if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.settingsKey, out XRGeneralSettingsPerBuildTarget perTarget) || perTarget == null)
        {
            perTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            Directory.CreateDirectory("Assets/XR");
            AssetDatabase.CreateAsset(perTarget, "Assets/XR/XRGeneralSettingsPerBuildTarget.asset");
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.settingsKey, perTarget, true);
        }

        if (!perTarget.HasSettingsForBuildTarget(buildTargetGroup))
        {
            perTarget.CreateDefaultSettingsForBuildTarget(buildTargetGroup);
            AssetDatabase.SaveAssets();
        }

        if (!perTarget.HasManagerSettingsForBuildTarget(buildTargetGroup))
        {
            perTarget.CreateDefaultManagerSettingsForBuildTarget(buildTargetGroup);
            AssetDatabase.SaveAssets();
        }

        var manager = perTarget.ManagerSettingsForBuildTarget(buildTargetGroup);
        if (manager != null)
        {
            XRPackageMetadataStore.AssignLoader(manager, "UnityEngine.XR.OpenXR.OpenXRLoader", buildTargetGroup);
            EditorUtility.SetDirty(manager);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[XRConfigurator] XR Plug-in Management configured (OpenXR loader assigned for Standalone).");
    }
}
