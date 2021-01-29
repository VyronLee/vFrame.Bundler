//------------------------------------------------------------
//        File:  BundleLoaderBase.cs
//       Brief:  BundleLoaderBase
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:08
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using UnityEngine;
using vFrame.Bundler.Base;
using vFrame.Bundler.Base.Pools;
using vFrame.Bundler.Exception;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Loaders
{
    public abstract class BundleLoaderBase : Reference
    {
        public static readonly Dictionary<string, AssetBundle> AssetBundleCache =
            new Dictionary<string, AssetBundle>();

        protected AssetBundle _assetBundle;

        protected string _path;
        protected List<string> _searchPaths;

        private bool _started;
        private bool _unloaded;

        protected BundlerOptions Options { get; private set; }

        public virtual AssetBundle AssetBundle {
            get { return _assetBundle; }
        }

        public string AssetBundlePath {
            get { return _path; }
        }

        public virtual bool IsLoading { get; protected set; }

        public List<BundleLoaderBase> Dependencies { get; set; }

        public virtual bool IsDone { get; protected set; }

        public bool IsStarted {
            get {
                return _started;
            }
        }

        public void Initialize(string path, List<string> searchPaths, BundlerOptions options) {
            _path = path;
            _searchPaths = searchPaths;
            _assetBundle = null;

            Options = options;
            Dependencies = ListPool<BundleLoaderBase>.Get();
        }

        public void Load() {
            if (IsDone || IsLoading) {
                return;
            }

            if (AssetBundleCache.ContainsKey(_path)) {
                throw new BundleException("AssetBundle already in cache: " + _path);
            }

            LoadProcess();
        }

        public void Unload() {
            Logger.LogInfo("Unload assetbundle: {0}", _path);

            if (_references > 0)
                throw new BundleException("Cannot unload, references: " + _references);

            if (_assetBundle) {
                _assetBundle.Unload(true);
                _assetBundle = null;
            }

            if (!AssetBundleCache.ContainsKey(_path)) {
                throw new BundleException("AssetBundle does not in cache: " + _path);
            }
            AssetBundleCache.Remove(_path);

            UnloadProcess();

            IsDone = false;

            ListPool<BundleLoaderBase>.Return(Dependencies);
        }

        public void ForceUnload() {
            Logger.LogVerbose("ForceUnload - {0}", _path);

            if (_assetBundle) {
                _assetBundle.Unload(true);
                _assetBundle = null;
            }

            AssetBundleCache.Remove(_path);

            foreach (var loader in Dependencies)
                loader.ForceUnload();

            ListPool<BundleLoaderBase>.Return(Dependencies);
        }

        public override void Retain() {
            RetainDependencies();

            base.Retain();

            Logger.LogVerbose("Retain loader: {0}, ref: {1}", _path, _references);
        }

        public override void Release() {
            ReleaseDependencies();

            base.Release();

            Logger.LogVerbose("Release loader: {0}, ref: {1}", _path, _references);
        }

        protected void RetainDependencies() {
            foreach (var loader in Dependencies)
                loader.Retain();
        }

        protected void ReleaseDependencies() {
            foreach (var loader in Dependencies)
                loader.Release();
        }

        private void LoadProcess() {
            if (!_started)
                RetainDependencies();
            _started = true;

            OnLoadProcess();
        }

        private void UnloadProcess() {
            if (!_unloaded)
                ReleaseDependencies();
            _unloaded = true;

            OnUnloadProcess();
        }

        protected abstract void OnLoadProcess();
        protected abstract void OnUnloadProcess();

        public override string ToString() {
            return string.Format("{4}: [path: {0}, started: {1}, done: {2}, unloaded: {3}]",
                _path, IsStarted, IsDone, _unloaded, GetType().Name);
        }
    }
}