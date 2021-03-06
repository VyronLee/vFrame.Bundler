﻿//------------------------------------------------------------
//        File:  LoadRequestSync.cs
//       Brief:  LoadRequestSync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using vFrame.Bundler.Exception;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Modes;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestSync : LoadRequest
    {
        public LoadRequestSync(ModeBase mode, string path, BundleLoaderBase bundleLoader)
            : base(mode, path, bundleLoader)
        {
        }

        public override bool IsDone
        {
            get { return _bundleLoader == null || _bundleLoader.IsDone; }
        }

        protected override void LoadInternal()
        {
            LoadRecursive(_bundleLoader);
            _bundleLoader.Release();
        }

        private void LoadRecursive(BundleLoaderBase bundleLoader)
        {
            // Load dependencies at first
            foreach (var dependency in bundleLoader.Dependencies)
                LoadRecursive(dependency);

            // Load target at last
            if (bundleLoader is BundleLoaderAsync)
                if (!bundleLoader.IsDone) {
                    throw new BundleMixLoaderException("Mix using async and sync loader is not supported.");
                }
            bundleLoader.Load();
        }
    }
}