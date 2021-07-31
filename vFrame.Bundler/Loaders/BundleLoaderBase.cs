//------------------------------------------------------------
//        File:  BundleLoaderBase.cs
//       Brief:  BundleLoaderBase
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:08
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
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
        private static int s_Indexer = 0;

        protected AssetBundle _assetBundle;

        protected string _path;
        protected List<string> _searchPaths;

        private bool _started;
        private bool _unloaded;
        private bool _done;

        private int _createdIndex = -1;
        private int _loadedIndex = -1;
        private DateTime _loadedTime = new DateTime(0);

        protected BundlerOptions Options { get; private set; }

        public virtual AssetBundle AssetBundle {
            get { return _assetBundle; }
        }

        public string AssetBundlePath {
            get { return _path; }
        }

        public int LoadedIndex {
            get { return _loadedIndex; }
        }

        public DateTime LoadedTime {
            get { return _loadedTime; }
        }

        public virtual bool IsLoading { get; protected set; }

        public List<BundleLoaderBase> Dependencies { get; set; }

        public bool Validate(ref List<BundleLoaderBase> errors) {
            var ret = true;
            foreach (var dependency in Dependencies) {
                ret &= dependency.Validate(ref errors);
            }

            if (!IsDone || IsUnloaded) {
                errors.Add(this);
                ret = false;
            }
            return ret;
        }

        public virtual bool IsDone {
            get {
                return _done;
            }
            protected set {
                _done = value;

                if (!value)
                    return;
                _loadedIndex = s_Indexer++;
                _loadedTime = DateTime.Now;
            }
        }

        public bool IsStarted {
            get {
                return _started;
            }
        }

        public bool IsUnloaded {
            get {
                return _unloaded;
            }
        }

        public void Initialize(string path, List<string> searchPaths, BundlerOptions options) {
            _path = path;
            _searchPaths = searchPaths;
            _assetBundle = null;
            _createdIndex = s_Indexer++;

            Options = options;
            Dependencies = ListPool<BundleLoaderBase>.Get();
        }

        public void Load() {
            if (IsDone || IsLoading) {
                return;
            }

            LoadProcess();
        }

        public void Unload() {
            Logger.LogInfo("Unload assetbundle: {0}", _path);

            if (_references > 0)
                throw new BundleException(
                    string.Format("Cannot unload, references: {0}, {1}", _references, this));

            if (_assetBundle) {
                _assetBundle.Unload(true);
                _assetBundle = null;
            }

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
            return string.Format(
                "{4}: [path: {0}, started: {1}, done: {2}, unloaded: {3}, refs: {5}, create index: {6}, loaded index: {7}, loaded time: {8:HH:mm:ss.ffff}]",
                _path, IsStarted, IsDone, _unloaded, GetType().Name, GetReferences(), _createdIndex, _loadedIndex, _loadedTime);
        }
    }
}