//------------------------------------------------------------
//        File:  BundleLoaderAsync.cs
//       Brief:  BundleLoaderAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:08
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.IO;
using UnityEngine;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Loader
{
    public class BundleLoaderAsync : BundleLoaderBase, IAsync
    {
        private AssetBundleCreateRequest _bundleLoadRequest;
        private IFileReaderRequest _fileReadRequest;

        public override AssetBundle AssetBundle
        {
            get
            {
                if (!_assetBundle)
                    _assetBundle = _bundleLoadRequest.assetBundle;
                return _assetBundle;
            }
        }

        public override bool IsDone
        {
            get { return !MoveNext(); }
        }

        public float Progress
        {
            get
            {
                if (_bundleLoadRequest == null)
                    return 0f;

                if (!_bundleLoadRequest.isDone)
                    return _bundleLoadRequest.progress;

                return 1f;
            }
        }

        public bool MoveNext()
        {
            if (_assetBundle)
                return false;

            if (BundlerSetting.kCustomFileReaderAsync != null)
            {
                if (_fileReadRequest == null)
                    _fileReadRequest = CreateFileReaderRequest();

                if (_fileReadRequest.MoveNext())
                    return true;

                _assetBundle =
                    AssetBundle.LoadFromMemory(_fileReadRequest.GetBytes()); // Problem with LoadFromMemoryAsync
            }
            else
            {
                if (_bundleLoadRequest == null)
                    _bundleLoadRequest = CreateBuiltinBundleLoadRequest();

                if (!_bundleLoadRequest.isDone || _bundleLoadRequest.progress < 1f)
                {
                    Logger.LogInfo("Bundle load request does not finished: {0}, progress: {1:0.00}",
                        _path, _bundleLoadRequest.progress);
                    return true;
                }

                Logger.LogInfo("Bundle load request finished: {0}", _path);

                _assetBundle = _bundleLoadRequest.assetBundle;
            }

            Logger.LogInfo("Add assetbundle to cache: " + _path);

            if (AssetBundleCache.ContainsKey(_path))
                throw new System.Exception("Assetbundle already in cache: " + _path);
            AssetBundleCache.Add(_path, _assetBundle);

            Logger.LogInfo("AssetBundle asynchronously loading finished, path: " + _path);

            IsLoading = false;
            return false;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        protected override bool LoadProcess()
        {
            Logger.LogInfo("Start asynchronously loading process: " + _path);

            IsLoading = true;

            return false;
        }

        private AssetBundleCreateRequest CreateBuiltinBundleLoadRequest()
        {
            Logger.LogInfo("Bundle load request does not exist, create it from file: {0}", _path);

            foreach (var basePath in _searchPaths)
            {
                var path = Path.Combine(basePath, _path);
                path = PathUtility.NormalizePath(path);

                try
                {
                    // Avoid throwing error messages.
                    if (PathUtility.IsFileInPersistentDataPath(path) && !File.Exists(path))
                    {
                        Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        continue;
                    }

                    var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
                    if (bundleLoadRequest != null)
                        return bundleLoadRequest;

                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
                catch
                {
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
            }

            throw new BundleLoadFailedException("Cannot load assetbundle: " + _path);
        }

        private IFileReaderRequest CreateFileReaderRequest()
        {
            Logger.LogInfo("File reader request does not exist, create it: {0}", _path);

            foreach (var basePath in _searchPaths)
            {
                var path = Path.Combine(basePath, _path);
                path = PathUtility.NormalizePath(path);

                try
                {
                    // Avoid throwing error messages.
                    if (PathUtility.IsFileInPersistentDataPath(path) && !File.Exists(path))
                    {
                        Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        continue;
                    }

                    var fileReader = BundlerSetting.kCustomFileReaderAsync.Clone();
                    return fileReader.ReadAllBytesAsync(path);
                }
                catch (System.Exception e)
                {
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, exception: {1}, searching next ... ",
                        path, e);
                }
            }

            throw new BundleLoadFailedException("Cannot load assetbundle: " + _path);
        }
    }
}