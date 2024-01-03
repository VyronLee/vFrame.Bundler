// ------------------------------------------------------------
//         File: BuildContextExtension.cs
//        Brief: BuildContextExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 19:51
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using vFrame.Bundler.Editor.Helper;

namespace vFrame.Bundler.Editor
{
    internal static class BuildContextExtension
    {
        public static string BuildBundlePath(this BuildContext context, string path) {
            var bundlePath = path.TrimEnd('/');
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = string.Format(context.BuildSettings.BundleFormatter, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        public static string BuildSharedBundlePath(this BuildContext context, string path) {
            var bundlePath = path.TrimEnd('/');
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = string.Format(context.BuildSettings.SharedBundleFormatter, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        public static string BuildSceneBundlePath(this BuildContext context, string path) {
            if (!AssetHelper.IsScene(path)) {
                ThrowHelper.ThrowArgumentException($"Scene file path required, got: {path}");
            }

            var bundlePath = path;
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = string.Format(context.BuildSettings.SceneBundleFormatter, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        public static string BuildSharedShaderBundlePath(this BuildContext context) {
            var bundlePath = context.BuildSettings.SeparatedShaderBundlePath.TrimEnd('/');
            bundlePath = HashBundlePathIfNeed(context, bundlePath);
            bundlePath = PathUtils.NormalizeAssetBundlePath(bundlePath);
            return bundlePath;
        }

        private static string HashBundlePathIfNeed(BuildContext context, string path) {
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