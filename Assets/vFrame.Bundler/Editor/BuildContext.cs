// ------------------------------------------------------------
//         File: BuildContext.cs
//        Brief: BuildContext.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-24 21:28
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    internal class BuildContext
    {
        // =====================
        //          Input
        // =====================
        public BundleBuildRules BuildRules { get; set; }
        public BundleBuildSettings BuildSettings { get; set; }

        // =====================
        //          Output
        // =====================
        public Dictionary<string, MainAssetInfo> MainAssetInfos { get; } = new Dictionary<string, MainAssetInfo>(); // Step 1
        public Dictionary<string, DependencyAssetInfo> DependencyAssetInfos { get; } = new Dictionary<string, DependencyAssetInfo>(); // Step 2,3
        public Dictionary<string, BundleInfo> BundleInfos { get; } = new Dictionary<string, BundleInfo>(); // Step 4
        public AssetBundleManifest AssetBundleManifest { get; set; } // Step 5
        public BundlerManifest BundlerManifest { get; set; } // Step 6
    }

    internal class MainAssetInfo
    {
        public string AssetPath { get; set; }
        public string BundlePath { get; set; }
    }

    internal class DependencyAssetInfo
    {
        public string AssetPath { get; set; }
        public string BundlePath { get; set; }
        public HashSet<string> ReferenceBundles { get; } = new HashSet<string>();
    }

    internal class BundleInfo
    {
        public string BundlePath { get; set; }
        public HashSet<string> AssetPaths { get; } = new HashSet<string>();
    }
}