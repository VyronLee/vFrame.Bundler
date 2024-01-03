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
    internal class Scene : LoaderHandler, IScene
    {
        private AsyncOperation _request;
        internal SceneLoader SceneLoader { get; private set; }

        public void Unload() {
            if (!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneLoader.SceneObject, true);
                return;
            }
            _request = SceneManager.UnloadSceneAsync(SceneLoader.SceneObject.path);
        }

        public void Activate() {
            if (!SceneLoader.SceneObject.IsValid()) {
                throw new InvalidOperationException("Scene invalid: " + SceneLoader.SceneObject.path);
            }
            SceneManager.SetActiveScene(SceneLoader.SceneObject);
        }

        public void Update() {
            if (null == _request || !_request.isDone) {
                return;
            }
            // TODO: Release bundles
        }

        internal override Loader Loader {
            set => SceneLoader = value as SceneLoader
                                 ?? throw new ArgumentException($"SceneLoader expected, got: {value.GetType()}");
        }
    }
}