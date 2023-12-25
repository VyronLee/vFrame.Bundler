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
    public class BundleBuildSettings
    {
        public string BundlePath { get; set; } = "Bundles";
        public string BuildRuleFilePath { get; set; } = "BundleRules.json";
        public string ManifestFileName { get; set; } = "Manifest.json";

        public string BundleFormatter { get; set; } = "{0}.ab";
        public string SharedBundleFormatter { get; set; } = "shared/shared_{0}.ab";
        public string SceneBundleFormatter { get; set; } = "{0}.scene.ab";

        public bool HashAssetBundlePath { get; set; } = true;
        public bool SeparateShaderBundle { get; set; } = true;
        public string SeparatedShaderBundleName { get; set; } = "shared/shared_shaders.ab";

        public BuildAssetBundleOptions AssetBundleBuildOptions { get; set; } =
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.DisableLoadAssetByFileName |
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension |
            BuildAssetBundleOptions.StrictMode;
    }
}