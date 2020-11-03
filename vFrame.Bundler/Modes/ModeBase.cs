//------------------------------------------------------------
//        File:  ModeBase.cs
//       Brief:  ModeBase
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:13
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.LoadRequests;
using vFrame.Bundler.Scenes;

namespace vFrame.Bundler.Modes
{
    public abstract class ModeBase
    {
        protected readonly BundlerManifest _manifest;
        protected readonly List<string> _searchPaths;

        protected ModeBase(BundlerManifest manifest, List<string> searchPaths)
        {
            _manifest = manifest;
            _searchPaths = searchPaths;
        }

        public abstract ILoadRequest Load(string path);
        public abstract ILoadRequestAsync LoadAsync(string path);

        public virtual void Collect()
        {
        }

        public virtual void DeepCollect()
        {
        }

        public abstract void Destroy();

        public virtual List<BundleLoaderBase> GetLoaders() {
            return new List<BundleLoaderBase>();
        }

        public virtual IScene GetScene(LoadRequest request, LoadSceneMode mode)
        {
            return new SceneSync(request.AssetPath, mode, request.Loader);
        }

        public virtual ISceneAsync GetSceneAsync(LoadRequest request, LoadSceneMode mode)
        {
            return new SceneAsync(request.AssetPath, mode, request.Loader);
        }

        public abstract IAsset GetAsset(LoadRequest request, Type type);
        public abstract IAssetAsync GetAssetAsync(LoadRequest request, Type type);
    }
}