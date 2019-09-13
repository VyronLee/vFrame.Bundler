//------------------------------------------------------------
//        File:  BundlerManifest.cs
//       Brief:  BundlerManifest
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:19
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using vFrame.Bundler.Utils;

namespace vFrame.Bundler
{
    [Serializable]
    public class BundlerManifest
    {
        public AssetsTable assets = new AssetsTable();
        public BundlesTable bundles = new BundlesTable();
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