// ------------------------------------------------------------
//         File: AssetDatabaseSceneLoader.cs
//        Brief: AssetDatabaseSceneLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 21:34
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Exception;
using Random = UnityEngine.Random;

namespace vFrame.Bundler
{
    internal class AssetDatabaseSceneLoaderAsync : SceneLoader
    {
        private UnityEngine.SceneManagement.Scene _sceneObject;
        private readonly int _startFrame;
        private readonly int _frameLength;
        private AsyncOperation _request;

        public AssetDatabaseSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

            _startFrame = Time.frameCount;
            _frameLength = Random.Range(
                bundlerContexts.Options.MinAsyncFrameCountWhenUsingAssetDatabase,
                bundlerContexts.Options.MaxAsyncFrameCountWhenUsingAssetDatabase);
        }

        public override float Progress {
            get {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying) {
                    return Mathf.Min((float)(Time.frameCount - _startFrame) / _frameLength, 1f);
                }
#endif

                if (null == _request) {
                    return 0f;
                }
                if (_request.isDone) {
                    return 1f;
                }
                return _request.progress;
            }
        }

        protected override void OnStart() {
#if UNITY_EDITOR
            var param = new LoadSceneParameters {loadSceneMode = LoadSceneMode};
            _request = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(AssetPath, param);
#else
            Facade.GetSystem<LogSystem>().LogError(
                $"{nameof(AssetDatabaseSceneLoader)} is not supported in runtime mode.");
            Abort();
#endif
        }

        protected override void OnStop() {

        }

        protected override void OnUpdate() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                if (Time.frameCount - _startFrame < _frameLength) {
                    return;
                }
            }
            else if (null == _request || !_request.isDone) {
                return;
            }
            ObtainSceneObject();
#endif
        }

        protected override void OnForceComplete() {
            throw new BundleNotSupportedException("Force complete async scene loader is not supported.");
        }

        private void ObtainSceneObject() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                var mode = LoadSceneMode == LoadSceneMode.Single
                    ? UnityEditor.SceneManagement.OpenSceneMode.Single
                    : UnityEditor.SceneManagement.OpenSceneMode.Additive;
                _sceneObject = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetPath, mode);
            }
            else {
                _sceneObject = SceneManager.GetSceneByPath(AssetPath);
            }
            if (_sceneObject != null) {
                Finish();
                return;
            }
            Facade.GetSystem<LogSystem>().LogError("Get scene instance failed: {0}", AssetPath);
            Abort();
#endif
        }

        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }
    }
}