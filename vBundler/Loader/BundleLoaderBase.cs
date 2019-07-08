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
using vBundler.Base;
using Logger = vBundler.Log.Logger;

namespace vBundler.Loader
{
    public abstract class BundleLoaderBase : Reference
    {
        protected static readonly Dictionary<string, AssetBundle> AssetBundleCache =
            new Dictionary<string, AssetBundle>();

        protected AssetBundle _assetBundle;

        protected string _path;
        protected List<string> _searchPaths;

        public virtual AssetBundle AssetBundle
        {
            get { return _assetBundle; }
        }

        public string AssetBundlePath
        {
            get { return _path; }
        }

        public virtual bool IsLoading { get; protected set; }

        public List<BundleLoaderBase> Dependencies { get; set; }

        public virtual bool IsDone { get; protected set; }

        public void Initialize(string path, List<string> searchPaths)
        {
            _path = path;
            _searchPaths = searchPaths;
            _assetBundle = null;

            Dependencies = new List<BundleLoaderBase>();
        }

        public void Load()
        {
            if (AssetBundleCache.TryGetValue(_path, out _assetBundle))
            {
                Logger.LogInfo("Load assetbundle from cache: " + _path);
                IsDone = true;
                return;
            }

            if (IsLoading)
                return;

            if (!(IsDone = LoadProcess()))
                return;

            Logger.LogInfo("Add assetbundle to cache: " + _path);

            if (AssetBundleCache.ContainsKey(_path))
                throw new System.Exception("Assetbundle already in cache: " + _path);
            AssetBundleCache.Add(_path, _assetBundle);
        }

        public void Unload()
        {
            Logger.LogInfo("Unload assetbundle: " + _path);

            if (_references > 0)
                throw new InvalidProgramException("Cannot unload, references: " + _references);

            if (_assetBundle)
            {
                _assetBundle.Unload(true);
                _assetBundle = null;
            }

            AssetBundleCache.Remove(_path);

            Dependencies.ForEach(v => v.Release());

            IsDone = !UnloadProcess();
        }

        public override void Retain()
        {
            Dependencies.ForEach(v => v.Retain());
            base.Retain();

            Logger.LogVerbose("Retain loader: {0}, ref: {1}", _path, _references);
        }

        public override void Release()
        {
            Dependencies.ForEach(v => v.Release());
            base.Release();

            Logger.LogVerbose("Release loader: {0}, ref: {1}", _path, _references);
        }

        protected abstract bool LoadProcess();

        protected virtual bool UnloadProcess()
        {
            return true;
        }
    }
}