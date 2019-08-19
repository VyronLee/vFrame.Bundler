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
using vBundler.Interface;
using vBundler.Loaders;
using vBundler.Modes;
using Object = UnityEngine.Object;

namespace vBundler.LoadRequests
{
    public class LoadRequest : ILoadRequest
    {
        protected readonly BundleLoaderBase _bundleLoader;
        protected readonly ModeBase _mode;
        protected readonly string _path;
        protected bool _finished;

        public LoadRequest(ModeBase mode, string path, BundleLoaderBase bundleLoader)
        {
            _path = path;
            _mode = mode;
            _bundleLoader = bundleLoader;

            _finished = _bundleLoader == null;
            if (!_finished) LoadInternal();
        }

        public BundleLoaderBase Loader
        {
            get { return _bundleLoader; }
        }

        public string AssetPath
        {
            get { return _path; }
        }

        public virtual bool IsDone
        {
            get { return _bundleLoader == null || _bundleLoader.IsDone; }
        }

        public IAsset GetAsset<T>() where T: Object
        {
            return _mode.GetAsset(this, typeof(T));
        }

        public IAsset GetAsset(Type type)
        {
            return _mode.GetAsset(this, type);
        }

        public IScene GetScene(LoadSceneMode mode)
        {
            return _mode.GetScene(this, mode);
        }

        public IAssetAsync GetAssetAsync<T>() where T: Object
        {
            return _mode.GetAssetAsync(this, typeof(T));
        }

        public IAssetAsync GetAssetAsync(Type type)
        {
            return _mode.GetAssetAsync(this, type);
        }

        public ISceneAsync GetSceneAsync(LoadSceneMode mode)
        {
            return _mode.GetSceneAsync(this, mode);
        }

        protected virtual void LoadInternal()
        {
        }
    }
}