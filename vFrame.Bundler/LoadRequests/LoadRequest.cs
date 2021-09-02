//------------------------------------------------------------
//        File:  LoadRequest.cs
//       Brief:  LoadRequest
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:09
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Base;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Modes;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.LoadRequests
{
    public abstract class LoadRequest : Reference, IDisposable, ILoadRequest
    {
        internal readonly BundlerContext _context;
        internal readonly ModeBase _mode;

        protected readonly BundleLoaderBase _bundleLoader;
        protected readonly string _path;

        internal LoadRequest(ModeBase mode, BundlerContext context, string path, BundleLoaderBase bundleLoader) {
            _path = path;
            _context = context;
            _mode = mode;
            _bundleLoader = bundleLoader;

            IsDone = _bundleLoader == null;

            if (!IsDone) {
                LoadInternal();
            }
        }

        public virtual void Dispose() {

        }

        private void LoadInternal() {
            OnLoadProcess();
        }

        protected abstract void OnLoadProcess();

        public override void Retain() {
            base.Retain();

            if (null != Loader) {
                Loader.Retain();
            }
        }

        public override void Release() {
            if (null != Loader) {
                Loader.Release();
            }
            base.Release();
        }

        public BundleLoaderBase Loader {
            get { return _bundleLoader; }
        }

        public string AssetPath {
            get { return _path; }
        }

        public bool IsDone { protected set; get; }

        public IAsset GetAsset<T>() where T : Object {
            return _mode.GetAsset(this, typeof(T));
        }

        public IAsset GetAsset(Type type) {
            return _mode.GetAsset(this, type);
        }

        public IScene GetScene(LoadSceneMode mode) {
            return _mode.GetScene(this, mode);
        }

        public override string ToString() {
            return string.Format("[{0} root loader: {1}]",
                GetType().Name,
                null == _bundleLoader ? "<None>" : _bundleLoader.ToString());
        }
    }
}