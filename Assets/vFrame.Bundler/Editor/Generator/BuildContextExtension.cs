// ------------------------------------------------------------
//         File: BuildContextExtension.cs
//        Brief: Computes final bundle file paths for normal, shared, scene and shader bundles,
//               applying optional MD5 path hashing and configured bundle name formatters.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:17:14
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Extension methods on <see cref="BuildContext"/> that compute final bundle file paths
    /// for normal, shared, scene and shader bundles.
    /// </summary>
    internal static class BuildContextExtension
    {
        /// <summary>
        /// Builds the final bundle path for a normal asset bundle.
        /// </summary>
        /// <param name="context">Build context providing path hashing and formatter settings.</param>
        /// <param name="path">Logical bundle path to transform.</param>
        /// <returns>The normalized bundle path after optional MD5 hashing and formatter application.</returns>
        public static string BuildBundlePath(this BuildContext context, string path)
        {
            var bundlePath = path.TrimEnd('/');
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = string.Format(context.BuildSettings.BundleFormatter, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        /// <summary>
        /// Builds the final bundle path for a shared dependency asset bundle.
        /// </summary>
        /// <param name="context">Build context providing path hashing and formatter settings.</param>
        /// <param name="path">Logical shared bundle path to transform.</param>
        /// <returns>The normalized shared bundle path after optional MD5 hashing and formatter application.</returns>
        public static string BuildSharedBundlePath(this BuildContext context, string path)
        {
            var bundlePath = path.TrimEnd('/');
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = string.Format(context.BuildSettings.SharedBundleFormatter, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        /// <summary>
        /// Builds the final bundle path for a scene asset bundle.
        /// </summary>
        /// <param name="context">Build context providing path hashing and formatter settings.</param>
        /// <param name="path">Scene asset path to transform.</param>
        /// <returns>The normalized scene bundle path after optional MD5 hashing and formatter application.</returns>
        /// <exception cref="BundleArgumentException">Thrown when <paramref name="path"/> is not a scene file path.</exception>
        public static string BuildSceneBundlePath(this BuildContext context, string path)
        {
            if (!AssetHelper.IsScene(path)) {
                ThrowHelper.ThrowArgumentException($"Scene file path required, got: {path}");
            }

            var bundlePath = path;
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = string.Format(context.BuildSettings.SceneBundleFormatter, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        /// <summary>
        /// Builds the final bundle path of the separated shared shader bundle.
        /// </summary>
        /// <param name="context">Build context providing the shader bundle path, hashing and formatter settings.</param>
        /// <returns>The normalized shader bundle path after optional MD5 hashing.</returns>
        public static string BuildSharedShaderBundlePath(this BuildContext context)
        {
            var bundlePath = context.BuildSettings.SeparatedShaderBundlePath.TrimEnd('/');
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        /// <summary>
        /// Returns the MD5 hash of <paramref name="path"/> as a lowercase hex string when path hashing
        /// is enabled in build settings; otherwise returns the path unchanged.
        /// </summary>
        /// <param name="context">Build context providing the hashing switch.</param>
        /// <param name="path">Bundle path to hash.</param>
        /// <returns>The hashed or original path, depending on build settings.</returns>
        private static string HashBundlePathIfNeed(BuildContext context, string path)
        {
            if (!context.BuildSettings.HashAssetBundlePath) {
                return path;
            }
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(path))) {
                using (var md5 = MD5.Create()) {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}