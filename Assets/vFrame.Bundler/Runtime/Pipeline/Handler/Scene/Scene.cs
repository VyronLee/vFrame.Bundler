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
        private bool _unloaded;
        private Loader _loader;

        internal SceneLoader SceneLoader => ((ILoaderHandler)this).Loader as SceneLoader
                                            ?? throw new ArgumentException("SceneLoader expected, got: "
                                                                           + (((ILoaderHandler)this).Loader?.GetType().Name ?? "null"));

        Loader ILoaderHandler.Loader {
            get => _loader;
            set {
                _loader = value;
                _loader.Retain();
            }
        }

        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        public UnloadOperation Unload() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneLoader.SceneObject, true);
                return UnloadOperation.Completed;
            }
#endif
            _request = SceneManager.UnloadSceneAsync(SceneLoader.AssetPath);
            return _unloadOperation ?? (_unloadOperation = new UnloadOperation());
        }

        public void Activate() {
            if (!SceneLoader.SceneObject.IsValid()) {
                throw new InvalidOperationException("Scene invalid: " + SceneLoader.AssetPath);
            }
            SceneManager.SetActiveScene(SceneLoader.SceneObject);
        }

        void ILoaderHandler.Update() {
            UpdateUnloadProcess();
        }

        private void UpdateUnloadProcess() {
            if (_unloaded) {
                return;
            }
            if (null == _request || !_request.isDone) {
                return;
            }
            _loader.Release();
            _unloadOperation?.SetDone(true);
            _unloaded = true;
        }

        public void Retain() {
            _loader?.Retain();
        }

        public void Release() {
            _loader?.Release();
        }

        [JsonSerializableProperty]
        public bool IsUnloaded => _unloaded;

        public override string ToString() {
            return $"[@TypeName: {GetType().Name}, AssetPath: {SceneLoader.AssetPath}]";
        }
    }
}