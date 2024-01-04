﻿//------------------------------------------------------------
//        File:  BundlerOptions.cs
//       Brief:  BundlerOptions
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:18
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System;

namespace vFrame.Bundler
{
    public class BundlerOptions
    {
        public BundlerMode Mode { get; set; } = BundlerMode.AssetBundle;
        public string[] SearchPaths = Array.Empty<string>();
        public IAssetBundleCreateRequestAdapter AssetBundleCreateRequestAdapter;
        public ILogHandler LogHandler;
        public int MinAsyncFrameCountOnSimulation { get; set; } = 1;
        public int MaxAsyncFrameCountOnSimulation { get; set; } = 10;
    }
}