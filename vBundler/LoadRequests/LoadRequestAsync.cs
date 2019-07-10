//------------------------------------------------------------
//        File:  LoadRequestAsync.cs
//       Brief:  LoadRequestAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using vBundler.Interface;
using vBundler.Loaders;
using vBundler.Modes;

namespace vBundler.LoadRequests
{
    public sealed class LoadRequestAsync : LoadRequest, ILoadRequestAsync
    {
        private readonly Stack<BundleLoaderBase> _loaders = new Stack<BundleLoaderBase>();
        private BundleLoaderBase _currentBundleLoader;
        private int _index;
        private readonly BundleLoaderBase _sourceBundleLoader;
        private int _total;

        public LoadRequestAsync(ModeBase mode, string path, BundleLoaderBase bundleLoader)
            : base(mode, path, bundleLoader)
        {
            _sourceBundleLoader = bundleLoader;
        }

        public bool MoveNext()
        {
            if (_finished)
                return false;

            if (_currentBundleLoader != null && !_currentBundleLoader.IsDone)
                return true;

            return LoadNext();
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        public override bool IsDone
        {
            get { return !MoveNext(); }
        }

        public float Progress
        {
            get
            {
                var loadingProgress = 0f;
                var loaderAsync = _currentBundleLoader as BundleLoaderAsync;
                if (loaderAsync != null)
                    loadingProgress = loaderAsync.Progress;

                return (_index + loadingProgress) / _total;
            }
        }

        private bool LoadNext()
        {
            // No others loader, finish immediately
            if (_loaders.Count <= 0)
            {
                _finished = true;
                _sourceBundleLoader.Release();
                return false;
            }

            ++_index;

            _currentBundleLoader = _loaders.Pop();
            _currentBundleLoader.Load();

            if (_currentBundleLoader.IsDone)
                return LoadNext();

            return true;
        }

        protected override void LoadInternal()
        {
            TravelRecursive(_bundleLoader);
            _total = _loaders.Count;
        }

        private void TravelRecursive(BundleLoaderBase bundleLoader)
        {
            _loaders.Push(bundleLoader);

            foreach (var dependency in bundleLoader.Dependencies)
                TravelRecursive(dependency);
        }
    }
}