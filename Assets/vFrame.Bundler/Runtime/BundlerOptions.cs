// ------------------------------------------------------------
//         File: BundlerOptions.cs
//        Brief: Construction options for a Bundler: mode, search paths, adapters, log handler, profiler address.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:04:50
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler
{
    public class BundlerOptions
    {
        public BundlerMode Mode { get; set; } = BundlerMode.AssetBundle;
        public string[] SearchPaths = Array.Empty<string>();
        public IAssetBundleCreateAdapter AssetBundleCreateAdapter;
        public ILogHandler LogHandler;
        public int MinAsyncFrameCountOnSimulation { get; set; } = 1;
        public int MaxAsyncFrameCountOnSimulation { get; set; } = 10;
        public string ListenAddress { get; set; } = "http://127.0.0.1:16667/";
    }
}