//------------------------------------------------------------
//        File:  BundlerMenu.cs
//       Brief:  BundlerMenu
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 10:40
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
using System.IO;
using UnityEditor;
using UnityEngine;
using vFrame.Bundler.Utils;

namespace vFrame.Bundler.Editor
{
    public static class BundlerMenu
    {
        [MenuItem("Assets/vFrame.Bundler/Generate Manifest")]
        public static void GenerateManifest()
        {
            var buildRuleFile = PathUtility.RelativeDataPathToAbsolutePath(BundlerDefaultBuildSettings.kBuildRuleFilePath);
            var relativeBuildRuleFile = PathUtility.AbsolutePathToRelativeProjectPath(buildRuleFile);
            var buildRuleAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeBuildRuleFile);
            var buildRule = JsonUtility.FromJson<BundlerBuildRule>(buildRuleAsset.text);

            BundlerGenerator.Instance.GenerateManifest(buildRule,
                PathUtility.Combine(BundlerDefaultBuildSettings.kBundlePath, BundlerDefaultBuildSettings.kManifestFileName));
        }

        [MenuItem("Assets/vFrame.Bundler/Generate AssetBundles(iOS)")]
        public static void GenerateAssetBundlesForIOS()
        {
            GenerateAssetBundles(BuildTarget.iOS);
        }

        [MenuItem("Assets/vFrame.Bundler/Generate AssetBundles(Android)")]
        public static void GenerateAssetBundlesForAndroid()
        {
            GenerateAssetBundles(BuildTarget.Android);
        }

        [MenuItem("Assets/vFrame.Bundler/Generate AssetBundles(Windows)")]
        public static void GenerateAssetBundlesForWindows()
        {
            GenerateAssetBundles(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Assets/vFrame.Bundler/Generate AssetBundles(MacOS)")]
        public static void GenerateAssetBundlesForMacOS()
        {
            GenerateAssetBundles(BuildTarget.StandaloneOSX);
        }

        [MenuItem("Assets/vFrame.Bundler/Re-Generate AssetBundle(iOS)")]
        public static void RegenerateAssetBundleForIOS()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.iOS);
        }

        [MenuItem("Assets/vFrame.Bundler/Re-Generate AssetBundle(Android)")]
        public static void RegenerateAssetBundleForAndroid()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.Android);
        }

        [MenuItem("Assets/vFrame.Bundler/Re-Generate AssetBundle(Windows)")]
        public static void RegenerateAssetBundleForWindows()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Assets/vFrame.Bundler/Re-Generate AssetBundle(MacOS)")]
        public static void RegenerateAssetBundleForMacOS()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            RegenerateAssetBundle(path, BuildTarget.StandaloneOSX);
        }

        private static void GenerateAssetBundles(BuildTarget platform)
        {
            var manifestFile =
                PathUtility.Combine(BundlerDefaultBuildSettings.kBundlePath, BundlerDefaultBuildSettings.kManifestFileName);
            var manifestFileFullPath = PathUtility.Combine(Application.streamingAssetsPath, manifestFile);

            if (!File.Exists(manifestFileFullPath))
            {
                EditorUtility.DisplayDialog("Error", "Please generate bundler manifest first.", "OK");
                return;
            }

            var manifestText = File.ReadAllText(manifestFileFullPath);
            var manifest = JsonUtility.FromJson<BundlerManifest>(manifestText);
            BundlerGenerator.Instance.GenerateAssetBundles(manifest, platform);
        }

        private static void RegenerateAssetBundle(string path, BuildTarget platform)
        {
            var bundleOutputFullPath =
                PathUtility.Combine(Application.streamingAssetsPath, BundlerDefaultBuildSettings.kBundlePath);
            var bundleOutputRelativePath = PathUtility.AbsolutePathToRelativeProjectPath(bundleOutputFullPath);
            var manifestFilePath = PathUtility.Combine(bundleOutputFullPath, BundlerDefaultBuildSettings.kManifestFileName);

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