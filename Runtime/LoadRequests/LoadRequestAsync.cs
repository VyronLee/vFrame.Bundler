//------------------------------------------------------------
//        File:  LoadRequestAsync.cs
//       Brief:  LoadRequestAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Logs;
using vFrame.Bundler.Modes;
using vFrame.Bundler.Utils;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestAsync : LoadRequest, ILoadRequestAsync, IAsyncProcessor
    {
        private List<BundleLoaderBase> _loaders;
        private List<BundleLoaderBase> _loading;

        internal LoadRequestAsync(ModeBase mode, BundlerContext context, string path, BundleLoaderBase bundleLoader)
            : base(mode, context, path, bundleLoader) {

        }

        public override void Dispose() {
            if (null != _context && null != _context.CoroutinePool) {
                AsyncRequestHelper.Uninstall(_context.CoroutinePool, this);
            }
            base.Dispose();
        }

        protected override void OnLoadProcess() {
            if (null == _bundleLoader) {
                return;
            }
            _loaders = GetAllLoaders();

            AsyncRequestHelper.Setup(_context.CoroutinePool, this);
        }

        public IEnumerator OnAsyncProcess() {
            IsStarted = true;

            var stopWatch = Stopwatch.StartNew();
            _bundleLoader.Retain();
            yield return TravelAndLoad();
            _bundleLoader.Release();

            var elapse = stopWatch.Elapsed.TotalSeconds;
            Logger.LogVerbose("LoadRequestAsync finished, cost: {0:0.0000}s, path: {1}", elapse, _bundleLoader.AssetBundlePath);

            IsDone = true;
        }

        public bool IsStarted { get; private set; }

        public float Progress {
            get {
                return CalculateLoadingProgress();
            }
        }

        public IAssetAsync GetAssetAsync<T>() where T : Object {
            return _mode.GetAssetAsync(this, typeof(T));
        }

        public IAssetAsync GetAssetAsync(Type type) {
            return _mode.GetAssetAsync(this, type);
        }

        public ISceneAsync GetSceneAsync(LoadSceneMode mode) {
            return _mode.GetSceneAsync(this, mode);
        }

        private List<BundleLoaderBase> GetAllLoaders() {
            var loaders = new HashSet<BundleLoaderBase>();
            GetAllLoadersInternal(_bundleLoader, ref loaders);
            return loaders.ToList();
        }

        private static void GetAllLoadersInternal(BundleLoaderBase root, ref HashSet<BundleLoaderBase> loaders) {
            foreach (var dependency in root.Dependencies) {
                GetAllLoadersInternal(dependency, ref loaders);
            }
            loaders.Add(root);
        }

        private IEnumerator TravelAndLoad() {
            var stopWatch = Stopwatch.StartNew();

            _loading = _loaders.ToList();

            // Check and start loaders
            foreach (var child in _loading) {
                if (!child.IsStarted) {
                    child.Load();
                }
            }

            // Wait until all loader finished.
            while (true) {
                for (var index = _loading.Count - 1; index >= 0; index--) {
                    var child = _loading[index];
                    if (!child.IsDone) {
                        continue;
                    }

                    var elapse = stopWatch.Elapsed.TotalSeconds;
                    Logger.LogVerbose("Load loader finished, cost: {0:0.0000}s, path: {1}", elapse, child.AssetBundlePath);

                    _loading.RemoveAt(index);
                }

                if (_loading.Count > 0) {
                    yield return null;
                    continue;
                }
                break;
            }
        }

        private float CalculateLoadingProgress() {
            if (null == _loading || null == _loaders) {
                return 0f;
            }

            var progress = 0f;
            foreach (var loader in _loading) {
                var loaderAsync = loader as BundleLoaderAsync;
                if (null != loaderAsync) {
                    progress += loaderAsync.Progress;
                }
                else {
                    progress += 1;
                }
            }

            var ret = ((float)_loaders.Count - _loading.Count + progress) / _loaders.Count;
            return ret;
        }

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current { get; private set; }
        public bool IsSetup { get; set; }
        public int ThreadHandle { get; set; }
    }
}