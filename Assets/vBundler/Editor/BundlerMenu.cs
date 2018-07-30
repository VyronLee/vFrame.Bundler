using System.IO;
using UnityEditor;
using UnityEngine;
using vBundler.Utils;

namespace vBundler.Editor
{
    public static class BundlerMenu
    {
        [MenuItem("Assets/vBundler/Generate Manifest")]
        private static void GenerateManifest()
        {
            var buildRuleFile = PathUtility.RelativeDataPathToFullPath(BundlerSetting.kDefualtBuildRuleFilePath);
            var relativeBuildRuleFile = PathUtility.FullPathToRelativeProjectPath(buildRuleFile);
            var buildRuleAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeBuildRuleFile);
            var buildRule = JsonUtility.FromJson<BundlerBuildRule>(buildRuleAsset.text);

            BundlerGenerator.Instance.GenerateManifest(buildRule,
                Path.Combine(BundlerSetting.kDefaultBundlePath, BundlerSetting.kDefaultManifestFileName));
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(iOS)")]
        private static void GenerateAssetBundlesForIOS()
        {
            GenerateAssetBundles(BuildTarget.iOS);
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(Android)")]
        private static void GenerateAssetBundlesForAndroid()
        {
            GenerateAssetBundles(BuildTarget.Android);
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(Windows)")]
        private static void GenerateAssetBundlesForWindows()
        {
            GenerateAssetBundles(BuildTarget.StandaloneWindows);
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(MacOS)")]
        private static void GenerateAssetBundlesForMacOS()
        {
            GenerateAssetBundles(BuildTarget.StandaloneOSXUniversal);
        }

        private static void GenerateAssetBundles(BuildTarget platform)
        {
            var manifestFile = Path.Combine(BundlerSetting.kDefaultBundlePath, BundlerSetting.kDefaultManifestFileName);
            var manifestFileFullPath = PathUtility.RelativeDataPathToFullPath(manifestFile);
            var manifestFileProjectPath = PathUtility.FullPathToRelativeProjectPath(manifestFileFullPath);
            var manifestAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(manifestFileProjectPath);

            if (!manifestAsset)
            {
                EditorUtility.DisplayDialog("Error", "Please generate vbundler manifest first.", "OK");
                return;
            }

            var manifest = JsonUtility.FromJson<BundlerManifest>(manifestAsset.text);
            BundlerGenerator.Instance.GenerateAssetBundles(manifest, platform);
        }
    }
}