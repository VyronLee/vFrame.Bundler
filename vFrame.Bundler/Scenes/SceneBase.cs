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
using System.Collections;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Base.Coroutine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vFrame.Bundler.Scenes
{
    public abstract class SceneBase : IScene
    {
        private static CoroutinePool _coroutinePool;

        private static CoroutinePool CoroutinePool {
            get { return _coroutinePool ?? (_coroutinePool = new CoroutinePool("BundleScene")); }
        }

        private readonly BundleLoaderBase _bundleLoader;
        protected readonly LoadSceneMode _mode;
        protected readonly string _path;
        protected readonly string _scenePath;
        protected readonly BundlerOptions _options;

        protected Scene Scene { get; set; }

        protected SceneBase(string path, LoadSceneMode mode, BundlerOptions options, BundleLoaderBase bundleLoader = null) {
            _path = path;
            _mode = mode;
            _options = options;
            _bundleLoader = bundleLoader;

            _scenePath = _path.Substring(7, _path.Length - 13); // Cut from "Assets/_____.unity"

            if (_bundleLoader != null && !_bundleLoader.IsDone)
                throw new BundleException("Loader hasn't finished!");

            LoadInternal();
            Retain();
        }

        public virtual bool IsDone { get; protected set; }

        public void Unload() {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                UnloadInternalInEditMode();
                return;
            }
#endif
            CoroutinePool.StartCoroutine(UnloadInternalInPlayMode());
        }

        private IEnumerator UnloadInternalInPlayMode() {
            yield return OnUnload();
            Release();
        }

#if UNITY_EDITOR
        private void UnloadInternalInEditMode() {
            OnUnloadInEditMode();
            Release();
        }
#endif

        public void Activate() {
            if (!Scene.IsValid())
                throw new InvalidOperationException("Scene invalid: " + Scene.path);

            SceneManager.SetActiveScene(Scene);
        }

        protected abstract void LoadInternal();
        protected abstract IEnumerator OnUnload();
#if UNITY_EDITOR
        protected abstract void OnUnloadInEditMode();
#endif

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