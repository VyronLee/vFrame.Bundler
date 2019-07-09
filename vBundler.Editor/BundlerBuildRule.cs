//------------------------------------------------------------
//        File:  BundlerBuildRule.cs
//       Brief:  BundlerBuildRule
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 10:40
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
using System;
using System.Collections.Generic;

namespace vBundler.Editor
{
    [Serializable]
    public static class PackType
    {
        public const int PackByFile = 1; // 使用searchPattern匹配到的单个文件名生成AB包
        public const int PackByDirectory = 2; // 使用指定的目录名生成AB包，仅searchPattern匹配到的文件会打进包里
        public const int PackBySubDirectory = 3; // 使用指定目录下的所有子目录名生成AB包，仅searchPattern匹配到的文件会打进包里
    }

    [Serializable]
    public class BundlerBuildRule
    {
        public List<BundleRule> rules = new List<BundleRule>();
    }

    [Serializable]
    public class BundleRule
    {
        public int depth;
        public string excludePattern = string.Empty;
        public int packType;
        public string path = string.Empty;
        public string searchPattern = string.Empty;
        public bool shared;
    }
}