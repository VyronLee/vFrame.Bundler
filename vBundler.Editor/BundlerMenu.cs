using System.IO;
using UnityEditor;
using UnityEngine;
using vBundler.Utils;

namespace vBundler.Editor
{
    public static class BundlerMenu
    {
        [MenuItem("Assets/vBundler/Generate Manifest")]
        public static void GenerateManifest()
        {
            var buildRuleFile = PathUtility.RelativeDataPathToAbsolutePath(BundlerBuildSetting.kDefaultBuildRuleFilePath);
            var relativeBuildRuleFile = PathUtility.AbsolutePathToRelativeProjectPath(buildRuleFile);
            var buildRuleAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeBuildRuleFile);
            var buildRule = JsonUtility.FromJson<BundlerBuildRule>(buildRuleAsset.text);

            BundlerGenerator.Instance.GenerateManifest(buildRule,
                PathUtility.Combine(BundlerBuildSetting.kDefaultBundlePath, BundlerBuildSetting.kDefaultManifestFileName));
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(iOS)")]
        public static void GenerateAssetBundlesForIOS()
        {
            GenerateAssetBundles(BuildTarget.iOS);
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(Android)")]
        public static void GenerateAssetBundlesForAndroid()
        {
            GenerateAssetBundles(BuildTarget.Android);
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(Windows)")]
        public static void GenerateAssetBundlesForWindows()
        {
            GenerateAssetBundles(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Assets/vBundler/Generate AssetBundles(MacOS)")]
        public static void GenerateAssetBundlesForMacOS()
        {
            GenerateAssetBundles(BuildTarget.StandaloneOSXUniversal);
        }

        [MenuItem("Assets/vBundler/Re-Generate AssetBundle(iOS)")]
        public static void RegenerateAssetBundleForIOS()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.iOS);
        }

        [MenuItem("Assets/vBundler/Re-Generate AssetBundle(Android)")]
        public static void RegenerateAssetBundleForAndroid()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.Android);
        }

        [MenuItem("Assets/vBundler/Re-Generate AssetBundle(Windows)")]
        public static void RegenerateAssetBundleForWindows()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Assets/vBundler/Re-Generate AssetBundle(MacOS)")]
        public static void RegenerateAssetBundleForMacOS()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.StandaloneOSXUniversal);
        }

        private static void GenerateAssetBundles(BuildTarget platform)
        {
            var manifestFile =
                PathUtility.Combine(BundlerBuildSetting.kDefaultBundlePath, BundlerBuildSetting.kDefaultManifestFileName);
            var manifestFileFullPath = PathUtility.Combine(Application.streamingAssetsPath, manifestFile);

            if (!File.Exists(manifestFileFullPath))
            {
                EditorUtility.DisplayDialog("Error", "Please generate vbundler manifest first.", "OK");
                return;
            }

            var manifestText = File.ReadAllText(manifestFileFullPath);
            var manifest = JsonUtility.FromJson<BundlerManifest>(manifestText);
            BundlerGenerator.Instance.GenerateAssetBundles(manifest, platform);
        }

        private static void RegenerateAssetBundle(string path, BuildTarget platform)
        {
            var bundleOutputFullPath =
                PathUtility.Combine(Application.streamingAssetsPath, BundlerBuildSetting.kDefaultBundlePath);
            var bundleOutputRelativePath = PathUtility.AbsolutePathToRelativeProjectPath(bundleOutputFullPath);
            var manifestFilePath = PathUtility.Combine(bundleOutputFullPath, BundlerBuildSetting.kDefaultManifestFileName);

            var bundleName = path.Substring(bundleOutputRelativePath.Length + 1);
            var jsonText = File.ReadAllText(manifestFilePath);
            var manifest = JsonUtility.FromJson<BundlerManifest>(jsonText);

            if (!manifest.bundles.ContainsKey(bundleName))
            {
                Debug.LogErrorFormat("Cannot found asset bundle in manifest: {0}", bundleName);
                return;
            }

            BundlerGenerator.Instance.RegenerateAssetBundle(manifest, bundleName, platform, bundleOutputRelativePath);
        }
    }
}