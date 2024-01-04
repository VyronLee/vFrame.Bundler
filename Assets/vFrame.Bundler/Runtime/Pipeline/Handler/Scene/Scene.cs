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

        internal SceneLoader SceneLoader => ((ILoaderHandler)this).Loader as SceneLoader
                   ?? throw new ArgumentException("SceneLoader expected, got: "
                       + (((ILoaderHandler)this).Loader?.GetType().Name ?? "null"));

        Loader ILoaderHandler.Loader { get; set; }

        public void Unload() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneLoader.SceneObject, true);
                return;
            }
#endif
            _request = SceneManager.UnloadSceneAsync(SceneLoader.AssetPath);
        }

        public void Activate() {
            if (!SceneLoader.SceneObject.IsValid()) {
                throw new InvalidOperationException("Scene invalid: " + SceneLoader.AssetPath);
            }
            SceneManager.SetActiveScene(SceneLoader.SceneObject);
        }

        void ILoaderHandler.Update() {
            if (null == _request || !_request.isDone) {
                return;
            }
            // TODO: Release bundles
        }

        public void Retain() {
            SceneLoader.Retain();
        }

        public void Release() {
            SceneLoader.Release();
        }
    }
}