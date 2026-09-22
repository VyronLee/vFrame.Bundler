// ------------------------------------------------------------
//         File: RuntimeSceneLoaderAsync.cs
//        Brief: Loads a scene asynchronously via SceneManager.LoadSceneAsync and validates the resulting Scene handle.
//               Force completion is not supported.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:55:59
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Asynchronously loads a scene via <see cref="SceneManager.LoadSceneAsync" /> and finishes once the
    ///     resulting <see cref="UnityEngine.SceneManagement.Scene" /> handle is valid; force completion is not supported.
    /// </summary>
    internal class RuntimeSceneLoaderAsync : SceneLoader
    {
        /// <summary>The in-flight async scene load operation, or <c>null</c> once stopped or aborted.</summary>
        private AsyncOperation _request;

        /// <summary>The scene handle captured after the async operation completes; invalid until then.</summary>
        private UnityEngine.SceneManagement.Scene _sceneObject;

        /// <summary>
        ///     Initializes the loader with the shared bundler state and the per-load scene context.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts.</param>
        /// <param name="loaderContexts">Contexts describing the scene to load.</param>
        protected RuntimeSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {
        }

        /// <summary>Load progress in [0, 1]; 0 before the operation starts, 1 once it is done.</summary>
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

        /// <summary>
        ///     Starts the async scene load with scene activation enabled, using the asset file name
        ///     without extension in the editor; aborts the loader when the operation cannot be created.
        /// </summary>
        protected override void OnStart()
        {
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

        /// <summary>Drops the reference to the in-flight async load operation.</summary>
        protected override void OnStop()
        {
            _request = null;
        }

        /// <summary>Captures the scene handle and finishes the loader once the async operation completes.</summary>
        protected override void OnUpdate()
        {
            if (null == _request) {
                return;
            }
            if (!_request.isDone) {
                return;
            }
            ObtainSceneObject();
        }

        /// <summary>
        ///     Not supported for asynchronous scene loading.
        /// </summary>
        /// <exception cref="BundleNotSupportedException">Always thrown.</exception>
        protected override void OnForceComplete()
        {
            throw new BundleNotSupportedException("Force complete async scene loader is not supported.");
        }

        /// <summary>
        ///     Captures the loaded scene handle by asset path and finishes the loader, or aborts
        ///     the loader when the scene instance cannot be obtained.
        /// </summary>
        private void ObtainSceneObject()
        {
            _sceneObject = SceneManager.GetSceneByPath(AssetPath);
            if (_sceneObject != null && _sceneObject.IsValid()) {
                Finish();
                return;
            }
            Facade.GetSystem<LogSystem>().LogError("Get scene instance failed: {0}", AssetPath);
            Abort();
        }

        /// <summary>Gets the loaded scene handle.</summary>
        /// <exception cref="BundleAssetNotReadyException">The loader has not finished.</exception>
        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }
    }
}