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
    internal abstract class ModeBase
    {
        protected readonly BundlerManifest _manifest;
        protected readonly List<string> _searchPaths;
        internal readonly BundlerContext _context;

        internal ModeBase(BundlerManifest manifest, List<string> searchPaths, BundlerContext context) {
            _manifest = manifest;
            _searchPaths = searchPaths;
            _context = context;
        }

        public abstract ILoadRequest Load(string path);
        public abstract ILoadRequestAsync LoadAsync(string path);

        public virtual void Collect() {
        }

        public virtual void DeepCollect() {
        }

        public abstract void Destroy();

        public virtual List<BundleLoaderBase> GetLoaders() {
            return new List<BundleLoaderBase>();
        }

        public virtual IScene GetScene(LoadRequest request, LoadSceneMode mode) {
            return new SceneSync(request.AssetPath, mode, _context, request.Loader);
        }

        public virtual ISceneAsync GetSceneAsync(LoadRequest request, LoadSceneMode mode) {
            return new SceneAsync(request.AssetPath, mode, _context, request.Loader);
        }

        public abstract IAsset GetAsset(LoadRequest request, Type type);
        public abstract IAssetAsync GetAssetAsync(LoadRequestAsync request, Type type);
    }
}