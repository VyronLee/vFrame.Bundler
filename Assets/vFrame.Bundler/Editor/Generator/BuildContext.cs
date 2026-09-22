// ------------------------------------------------------------
//         File: BuildContext.cs
//        Brief: Shared context passed between pipeline tasks: carries build rules/settings as input and
//               accumulates per-step outputs (asset infos, bundle infos, manifests).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:19:35
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Shared context exchanged between bundle build pipeline tasks. Holds the build rules and
    ///     settings as input, and accumulates per-step outputs: main/dependency asset infos, bundle
    ///     infos, and the resulting manifests.
    /// </summary>
    internal class BuildContext
    {
        // =====================
        //          Input
        // =====================

        /// <summary>Grouping and packing rules that decide how assets are assigned to bundles.</summary>
        public BundleBuildRules BuildRules { get; set; }

        /// <summary>Build settings controlling output paths, naming and compression of bundles.</summary>
        public BundleBuildSettings BuildSettings { get; set; }

        // =====================
        //          Output
        // =====================

        /// <summary>Main assets discovered in step 1, keyed by asset path.</summary>
        public Dictionary<string, MainAssetInfo> MainAssetInfos { get; } = new Dictionary<string, MainAssetInfo>(); // Step 1

        /// <summary>Dependency assets analyzed in steps 2-3, keyed by asset path.</summary>
        public Dictionary<string, DependencyAssetInfo> DependencyAssetInfos { get; } = new Dictionary<string, DependencyAssetInfo>(); // Step 2,3

        /// <summary>Bundles assembled in step 4, keyed by bundle path.</summary>
        public Dictionary<string, BundleInfo> BundleInfos { get; } = new Dictionary<string, BundleInfo>(); // Step 4

        /// <summary>Unity manifest produced by the AssetBundle build in step 5; null for simulation.</summary>
        public AssetBundleManifest AssetBundleManifest { get; set; } // Step 5

        /// <summary>Bundler manifest generated from the build outcomes in step 7 and written to disk.</summary>
        public BundlerManifest BundlerManifest { get; set; } // Step 7
    }

    /// <summary>Describes a main asset (directly matched by MainRules) and the bundle it packs into.</summary>
    internal class MainAssetInfo
    {
        /// <summary>Source asset path in the project.</summary>
        public string AssetPath { get; set; }

        /// <summary>Normalized target bundle path this asset is packed into.</summary>
        public string BundlePath { get; set; }
    }

    /// <summary>Describes a shared dependency asset and the bundles that reference it.</summary>
    internal class DependencyAssetInfo
    {
        /// <summary>Source asset path in the project.</summary>
        public string AssetPath { get; set; }

        /// <summary>Normalized target bundle path this dependency is packed into.</summary>
        public string BundlePath { get; set; }

        /// <summary>Paths of all bundles that reference this dependency asset.</summary>
        public HashSet<string> ReferenceBundles { get; } = new HashSet<string>();
    }

    /// <summary>Describes a single output bundle and the assets packed into it.</summary>
    internal class BundleInfo
    {
        /// <summary>Normalized bundle path used as the bundle name.</summary>
        public string BundlePath { get; set; }

        /// <summary>Paths of all assets packed into this bundle.</summary>
        public HashSet<string> AssetPaths { get; } = new HashSet<string>();
    }
}