//------------------------------------------------------------
//        File:  SceneBase.cs
//       Brief:  SceneBase
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:15
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;

namespace vFrame.Bundler.Scenes
{
    public abstract class SceneBase : IScene
    {
        private readonly BundleLoaderBase _bundleLoader;
        protected readonly LoadSceneMode _mode;
        protected readonly string _path;
        protected readonly string _scenePath;

        protected SceneBase(string path, LoadSceneMode mode, BundleLoaderBase bundleLoader = null) {
            _path = path;
            _mode = mode;
            _bundleLoader = bundleLoader;

            _scenePath = _path.Substring(7, _path.Length - 13); // Cut from "Assets/_____.unity"

            if (_bundleLoader != null && !_bundleLoader.IsDone)
                throw new BundleException("Loader hasn't finished!");

            LoadInternal();
            Retain();
        }

        public virtual bool IsDone { get; protected set; }

        public void Unload() {
            SceneManager.UnloadSceneAsync(_scenePath);
            Release();
        }

        public void Activate() {
            var scene = SceneManager.GetSceneByPath(_path);
            if (scene == null)
                throw new BundleInstanceNotFoundException("No such scene instance: " + _path);

            SceneManager.SetActiveScene(scene);
        }

        protected abstract void LoadInternal();

        public void Retain() {
            if (_bundleLoader != null)
                _bundleLoader.Retain();
        }

        public void Release() {
            if (_bundleLoader != null)
                _bundleLoader.Release();
        }
    }
}