// ------------------------------------------------------------
//         File: BundlerOptions.cs
//        Brief: Construction options for a Bundler: loading mode, search paths, bundle adapter, log handler, profiler
//               listen address.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:55:20
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Construction options for a Bundler: loading mode, search paths, bundle adapter, log handler, profiler listen address.
    /// </summary>
    public class BundlerOptions
    {
        /// <summary>
        ///     Asset loading backend: AssetDatabase (editor-only), Resources, or AssetBundle (requires a manifest).
        /// </summary>
        public BundlerMode Mode { get; set; } = BundlerMode.AssetBundle;

        /// <summary>
        ///     Directories searched in order when resolving bundle files; put patch/hotfix paths first.
        /// </summary>
        public string[] SearchPaths = Array.Empty<string>();

        /// <summary>
        ///     Creates/loads AssetBundle instances in AssetBundle mode; defaults to the internal file-based adapter when null.
        /// </summary>
        public IAssetBundleCreateAdapter AssetBundleCreateAdapter;

        /// <summary>
        ///     Custom log sink; falls back to the default handler when null.
        /// </summary>
        public ILogHandler LogHandler;

        /// <summary>
        ///     Simulation mode only: inclusive lower bound of the random frame count an async load takes.
        /// </summary>
        public int MinAsyncFrameCountOnSimulation { get; set; } = 1;

        /// <summary>
        ///     Simulation mode only: exclusive upper bound of the random frame count an async load takes.
        /// </summary>
        public int MaxAsyncFrameCountOnSimulation { get; set; } = 10;

        /// <summary>
        ///     HTTP endpoint the runtime profiler listens on for the Bundler Profiler window.
        /// </summary>
        public string ListenAddress { get; set; } = "http://127.0.0.1:16667/";
    }
}