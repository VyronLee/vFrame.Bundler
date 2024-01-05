// ------------------------------------------------------------
//         File: Scene.cs
//        Brief: Scene.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 23:18
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    public class Scene : ILoaderHandler
    {
        private AsyncOperation _request;
        private UnloadOperation _unloadOperation;

        internal SceneLoader SceneLoader => ((ILoaderHandler)this).Loader as SceneLoader
                                            ?? throw new ArgumentException("SceneLoader expected, got: "
                                                                           + (((ILoaderHandler)this).Loader?.GetType().Name ?? "null"));

        Loader ILoaderHandler.Loader { get; set; }
        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        public UnloadOperation Unload() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneLoader.SceneObject, true);
                return UnloadOperation.Completed;
            }
#endif
            _request = SceneManager.UnloadSceneAsync(SceneLoader.AssetPath);
            _unloadOperation = new UnloadOperation();
            return _unloadOperation;
        }

        public void Activate(bool setAsActiveScene = true) {
            Retain();

            if (!setAsActiveScene) {
                return;
            }
            if (!SceneLoader.SceneObject.IsValid()) {
                throw new InvalidOperationException("Scene invalid: " + SceneLoader.AssetPath);
            }
            SceneManager.SetActiveScene(SceneLoader.SceneObject);
        }

        void ILoaderHandler.Update() {
            if (null == _request || !_request.isDone) {
                return;
            }
            Release();

            _unloadOperation?.SetDone(true);
        }

        public void Retain() {
            SceneLoader.Retain();
        }

        public void Release() {
            SceneLoader.Release();
        }
    }
}