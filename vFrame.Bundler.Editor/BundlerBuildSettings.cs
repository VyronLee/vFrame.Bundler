//------------------------------------------------------------
//        File:  BundlerDefaultBuildSetting.cs
//       Brief:  BundlerDefaultBuildSetting
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 10:39
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using UnityEditor;

namespace vFrame.Bundler.Editor
{
    public static class BundlerBuildSettings
    {
        public static string kBundlePath = "Bundles";
        public static string kBuildRuleFilePath = "BundleRules.json";
        public static string kManifestFileName = "Manifest.json";

        public static string kBundleFormatter = "{0}.ab";
        public static string kSharedBundleFormatter = "shared/shared_{0}.ab";
        public static string kSceneBundleFormatter = "{0}.scene.ab";

        public static bool kHashAssetBundlePath = true;
        public static bool kHashSharedBundle = true;
        public static bool kSeparateShaderBundle = true;
        public static string kSeparatedShaderBundleName = "shared/shared_shaders.ab";

        public static string kModePreferenceKey = "vFrameBundlerModePreferenceKey";
        public static string kLogLevelPreferenceKey = "vFrameBundlerLogLevelPreferenceKey";

        public static readonly BuildAssetBundleOptions kAssetBundleBuildOptions =
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.StrictMode;
    }
}