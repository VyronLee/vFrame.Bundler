// ------------------------------------------------------------
//         File: BundleBuildSettings.cs
//        Brief: Settings for an AssetBundle build: output paths, bundle naming, shader separation, and build options.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:19:48
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using UnityEditor;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Editor-side settings that configure an AssetBundle build, including the output
    /// directory, generated manifest file name, bundle name formatters, shader bundle
    /// separation, and the default <see cref="BuildAssetBundleOptions"/>.
    /// </summary>
    public class BundleBuildSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to build in Unity's dry-run mode,
        /// validating the build without producing full bundle files.
        /// </summary>
        public bool DryRun { get; set; } = false;
        /// <summary>
        /// Gets or sets the output directory where the built AssetBundles are written.
        /// </summary>
        public string BundlePath { get; set; } = "Bundles";
        /// <summary>
        /// Gets or sets the file name of the generated bundler manifest (default:
        /// "BundlerManifest.json").
        /// </summary>
        public string ManifestFileName { get; set; } = "BundlerManifest.json";
        /// <summary>
        /// Gets or sets the target platform the AssetBundles are built for. Defaults to
        /// the editor's currently active build target.
        /// </summary>
        public BuildTarget BuildTarget { get; set; } = EditorUserBuildSettings.activeBuildTarget;

        /// <summary>
        /// Gets or sets the bundle name format string for regular asset bundles, where
        /// {0} is the bundle name (default: "{0}.ab").
        /// </summary>
        public string BundleFormatter { get; set; } = "{0}.ab";
        /// <summary>
        /// Gets or sets the bundle name format string for shared (dependency-grouped)
        /// bundles, where {0} is the shared bundle name (default: "shared/shared_{0}.ab").
        /// </summary>
        public string SharedBundleFormatter { get; set; } = "shared/shared_{0}.ab";
        /// <summary>
        /// Gets or sets the bundle name format string for scene bundles, where {0} is the
        /// scene name (default: "{0}.scene.ab").
        /// </summary>
        public string SceneBundleFormatter { get; set; } = "{0}.scene.ab";

        /// <summary>
        /// Gets or sets a value indicating whether a content hash is appended to bundle
        /// file paths for cache busting.
        /// </summary>
        public bool HashAssetBundlePath { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether shaders are extracted from other assets
        /// into a dedicated shared shader bundle.
        /// </summary>
        public bool SeparateShaderBundle { get; set; } = true;
        /// <summary>
        /// Gets or sets the bundle path used for the separated shared shader bundle when
        /// <see cref="SeparateShaderBundle"/> is enabled.
        /// </summary>
        public string SeparatedShaderBundlePath { get; set; } = "shared/shared_shaders.ab";

        /// <summary>
        /// Gets or sets the options passed to the Unity build pipeline. Defaults to
        /// chunk-based LZ4 compression, deterministic build outputs, disabled loading by
        /// file name, and strict mode.
        /// </summary>
        public BuildAssetBundleOptions AssetBundleBuildOptions { get; set; } =
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.DisableLoadAssetByFileName |
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension |
            BuildAssetBundleOptions.StrictMode;
    }
}