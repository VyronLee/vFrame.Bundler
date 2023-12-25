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
        PackBySingleFile = 1, // Generate AB package using a single file name matched by searchPattern
        PackByTopDirectory = 2, // Use the specified directory name to generate an AB package.
                             // Only files matched by searchPattern and excludePattern will be put into the package.
        PackByAllDirectories = 3 // Use all directory names in the specified directory to generate an AB package.
                               // Only files matched by searchPattern and excludePattern will be included in the package.
    }

    [Serializable]
    public class BundleBuildRules
    {
        public List<MainBundleRule> MainRules { get; set; } = new List<MainBundleRule>();
        public List<AutoGroupRule> GroupRules { get; set; } = new List<AutoGroupRule>();
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
    }
}