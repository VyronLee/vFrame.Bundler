// ------------------------------------------------------------
//         File: BundleBuildSettings.cs
//        Brief: Editor AssetBundle build settings: output path, manifest file name, bundle name
//               formatters, shader separation, and BuildAssetBundleOptions defaults.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:04:27
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================



using UnityEditor;

namespace vFrame.Bundler
{
    public class BundleBuildSettings
    {
        public bool DryRun { get; set; } = false;
        public string BundlePath { get; set; } = "Bundles";
        public string ManifestFileName { get; set; } = "BundlerManifest.json";
        public BuildTarget BuildTarget { get; set; } = EditorUserBuildSettings.activeBuildTarget;

        public string BundleFormatter { get; set; } = "{0}.ab";
        public string SharedBundleFormatter { get; set; } = "shared/shared_{0}.ab";
        public string SceneBundleFormatter { get; set; } = "{0}.scene.ab";

        public bool HashAssetBundlePath { get; set; } = true;
        public bool SeparateShaderBundle { get; set; } = true;
        public string SeparatedShaderBundlePath { get; set; } = "shared/shared_shaders.ab";

        public BuildAssetBundleOptions AssetBundleBuildOptions { get; set; } =
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.DisableLoadAssetByFileName |
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension |
            BuildAssetBundleOptions.StrictMode;
    }
}