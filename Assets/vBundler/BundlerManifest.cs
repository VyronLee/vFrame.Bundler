using System;
using System.Collections.Generic;
using vBundler.Utils;

namespace vBundler
{
    [Serializable]
    public class BundlerManifest
    {
        public AssetsTable assets = new AssetsTable();
        public BundlesTable bundles = new BundlesTable();
        public string bundlePath = BundlerSetting.kDefaultBundlePath;
    }

    [Serializable]
    public class AssetData
    {
        public string bundle;
        public string name;
    }

    [Serializable]
    public class BundleData
    {
        public List<string> dependencies = new List<string>();
        public string md5;
        public string name;
    }

    [Serializable]
    public class AssetsTable : SerializableDictionary<string, AssetData>
    {
    }

    [Serializable]
    public class BundlesTable : SerializableDictionary<string, BundleData>
    {
    }
}