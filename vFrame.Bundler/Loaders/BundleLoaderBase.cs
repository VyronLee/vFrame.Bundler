﻿//------------------------------------------------------------
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
using System.IO;
using UnityEngine;
using vFrame.Bundler.Base;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Utils.Pools;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Loaders
{
    public abstract class BundleLoaderBase : Reference
    {
        protected static readonly Dictionary<string, AssetBundle> AssetBundleCache =
            new Dictionary<string, AssetBundle>();

        protected AssetBundle _assetBundle;
        protected Stream _fileStream;

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

            Dependencies = ListPool<BundleLoaderBase>.Get();
        }

        public void Load()
        {
            if (AssetBundleCache.TryGetValue(_path, out _assetBundle))
            {
                Logs.Logger.LogInfo("Load assetbundle from cache: {0}", _path);
                IsDone = true;
                return;
            }

            if (IsLoading)
                return;

            if (!(IsDone = LoadProcess()))
                return;

            Logs.Logger.LogInfo("Add assetbundle to cache: {0}", _path);

            if (AssetBundleCache.ContainsKey(_path))
                throw new System.Exception("Assetbundle already in cache: " + _path);
            AssetBundleCache.Add(_path, _assetBundle);
        }

        public void Unload()
        {
            Logs.Logger.LogInfo("Unload assetbundle: {0}", _path);

            if (_references > 0)
                throw new BundleException("Cannot unload, references: " + _references);

            if (_assetBundle)
            {
                _assetBundle.Unload(true);
                _assetBundle = null;
            }

            if (null != _fileStream) {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }

            AssetBundleCache.Remove(_path);

            IsDone = !UnloadProcess();

            foreach (var loader in Dependencies)
                loader.Release();

            ListPool<BundleLoaderBase>.Return(Dependencies);
        }

        public override void Retain()
        {
            foreach (var loader in Dependencies)
                loader.Retain();

            base.Retain();

            Logs.Logger.LogVerbose("Retain loader: {0}, ref: {1}", _path, _references);
        }

        public override void Release()
        {
            foreach (var loader in Dependencies)
                loader.Release();

            base.Release();

            Logs.Logger.LogVerbose("Release loader: {0}, ref: {1}", _path, _references);
        }

        protected abstract bool LoadProcess();

        protected virtual bool UnloadProcess()
        {
            return true;
        }
    }
}
