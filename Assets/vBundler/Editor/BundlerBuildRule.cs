using System;
using System.Collections.Generic;

namespace vBundler.Editor
{
    [Serializable]
    public static class PackType
    {
        public const int PackByDirectory = 1;
        public const int PackByFile = 2;
    }

    [Serializable]
    public class BundlerBuildRule
    {
        public List<BundleRule> rules = new List<BundleRule>();
    }

    [Serializable]
    public class BundleRule
    {
        public int packType;
        public string path;
        public string searchPattern;
    }
}