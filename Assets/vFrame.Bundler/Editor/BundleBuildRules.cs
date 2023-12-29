// ------------------------------------------------------------
//         File: BundlerBuildRule.cs
//        Brief: AssetBundle build rule structure definition
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-19 16:28
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System;
using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    public enum PackType
    {
        PackBySingleFile = 1,
        PackByAllFiles = 2,
        PackByTopDirectory = 3,
        PackByAllDirectories = 4
    }

    [Serializable]
    public class BundleBuildRules
    {
        public List<MainBundleRule> MainRules { get; } = new List<MainBundleRule>();
        public List<AutoGroupRule> GroupRules { get; } = new List<AutoGroupRule>();
    }

    [Serializable]
    public class MainBundleRule
    {
        /// <summary>
        /// Packing method type, see PackType definition
        /// </summary>
        public string PackType { get; set; }

        /// <summary>
        /// Search path
        /// </summary>
        public string SearchPath { get; set; } = "";

        /// <summary>
        /// Regular expression, only resource files that meet the conditions will be
        /// included in the AB package
        /// </summary>
        public string Include { get; set; } = ".+";

        /// <summary>
        /// Regular expression, files that meet the conditions will be excluded and will not
        /// enter the AB package
        /// </summary>
        public string Exclude { get; set; } = "";
    }

    [Serializable]
    public class AutoGroupRule
    {
        public string Include { get; set; } = "(.+)";
        public string Exclude { get; set; } = "";

        public static AutoGroupRule Fallback { get; } = new AutoGroupRule { Include = "(.+)" };
    }
}