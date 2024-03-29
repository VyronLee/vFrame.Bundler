// ------------------------------------------------------------
//         File: RuntimeSceneLoader.cs
//        Brief: RuntimeSceneLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 21:59
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    internal class RuntimeSceneLoaderAsync : SceneLoader
    {
        private AsyncOperation _request;
        private UnityEngine.SceneManagement.Scene _sceneObject;

        protected RuntimeSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {
        }

        [JsonSerializableProperty]
        public override float Progress {
            get {
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
            var sceneName = Path.GetFileNameWithoutExtension(AssetPath);
            _request = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode);
#else
            _request = SceneManager.LoadSceneAsync(AssetPath, LoadSceneMode);  // Less GC
#endif
            if (null != _request) {
                _request.allowSceneActivation = true;
                return;
            }
            Facade.GetSystem<LogSystem>().LogError("Create load scene AsyncOperation failed: {0}", AssetPath);
            Abort();
        }

        protected override void OnStop() {
            _request = null;
        }

        protected override void OnUpdate() {
            if (null == _request) {
                return;
            }
            if (!_request.isDone) {
                return;
            }
            ObtainSceneObject();
        }

        protected override void OnForceComplete() {
            throw new BundleNotSupportedException("Force complete async scene loader is not supported.");
        }

        private void ObtainSceneObject() {
            _sceneObject = SceneManager.GetSceneByPath(AssetPath);
            if (_sceneObject != null && _sceneObject.IsValid()) {
                Finish();
                return;
            }
            Facade.GetSystem<LogSystem>().LogError("Get scene instance failed: {0}", AssetPath);
            Abort();
        }

        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }
    }
}