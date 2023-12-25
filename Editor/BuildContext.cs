// ------------------------------------------------------------
//         File: BuildContext.cs
//        Brief: BuildContext.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-24 21:28
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    internal class BuildContext
    {
        public BundleBuildRules BuildRules { get; set; }
        public BundleBuildSettings BuildSettings { get; set; }

        public List<MainAssetInfo> MainAssetInfos { get; set; } = new List<MainAssetInfo>();
        public List<DependencyAssetInfo> DependencyAssetInfos { get; set; } = new List<DependencyAssetInfo>();
    }

    internal class MainAssetInfo
    {
        public string AssetPath { get; set; }
        public string BundlePath { get; set; }
    }

    internal class DependencyAssetInfo
    {
        public string AssetPath { get; set; }
        public HashSet<string> ReferenceInBundles { get; set; } = new HashSet<string>();
    }
}