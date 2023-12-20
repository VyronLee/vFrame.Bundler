// ------------------------------------------------------------
//         File: BundlerBuildRule.cs
//        Brief: BundlerBuildRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-19 16:28
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace vFrame.Bundler.Editor
{
    public enum PackType
    {
        PackByFile = 1, // Generate AB package using a single file name matched by searchPattern
        PackByDirectory = 2, // Use the specified directory name to generate an AB package.
                             // Only files matched by searchPattern and excludePattern will be put into the package.
        PackBySubDirectory = 3 // Use all subdirectory names in the specified directory to generate an AB package.
                               // Only files matched by searchPattern and excludePattern will be included in the package.
    }

    [Serializable]
    public class BundlerBuildRule
    {
        public List<BundleRule> rules = new();
        public List<ManagedFileRule> managed = new();
    }

    [Serializable]
    public class BundleRule
    {
        public string packType;
        public string path = string.Empty;
        public string includePattern = ".+"; // Include all files by default
        public string excludePattern = ""; // Exclude nothing by default
        public int depth;
        public bool shared;
    }

    [Serializable]
    public class ManagedFileRule
    {
#pragma warning disable 0649

        public string directory = string.Empty;
        public string pattern = string.Empty;

#pragma warning restore 0649
    }
}