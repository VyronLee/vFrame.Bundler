//------------------------------------------------------------
//        File:  BundlerDefaultBuildSetting.cs
//       Brief:  BundlerDefaultBuildSetting
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 10:39
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
namespace vFrame.Bundler.Editor
{
    public class BundlerDefaultBuildSettings
    {
        public static string kBundlePath = "Bundles";
        public static string kBuildRuleFilePath = "BundleRules.json";
        public static string kManifestFileName = "Manifest.json";

        public static string kBundleFormatter = "{0}";
        public static string kSharedBundleFormatter = "shared/shared_{0}";
        public static string kSceneBundleFormatter = "{0}.scene";

        public static bool kHashAssetBundlePath = true;

        public static string kModePreferenceKey = "vFrameBundlerModePreferenceKey";
        public static string kLogLevelPreferenceKey = "vFrameBundlerLogLevelPreferenceKey";
    }
}