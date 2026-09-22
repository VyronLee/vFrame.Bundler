// ------------------------------------------------------------
//         File: AssetDatabaseSceneLoaderAsync.cs
//        Brief: Editor-mode async scene loader for AssetDatabase mode: async load in play mode, sync open in edit
//               mode; logs an error and aborts in player builds.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:37:57
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    /// Editor-mode asynchronous scene loader for AssetDatabase mode. Loads via the editor scene manager
    /// asynchronously in play mode, opens the scene synchronously in edit mode, and aborts with an error
    /// log in player builds.
    /// </summary>
    internal class AssetDatabaseSceneLoaderAsync : SceneLoader
    {
        /// <summary>The scene instance captured once loading completes; default until then.</summary>
        private UnityEngine.SceneManagement.Scene _sceneObject;

        /// <summary>Unused legacy field; never read or written.</summary>
        private readonly int _startFrame;

        /// <summary>Unused legacy field; never read or written.</summary>
        private readonly int _frameLength;

        /// <summary>The async operation driving the play-mode load; null in edit mode or before the load starts.</summary>
        private AsyncOperation _request;

        /// <summary>
        /// Creates the loader from the shared bundler contexts and the loader creation contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts.</param>
        /// <param name="loaderContexts">Contexts supplying the target scene asset path and load mode.</param>
        public AssetDatabaseSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>
        /// Gets the load progress: 0 before the request exists, 1 when it is done,
        /// otherwise the underlying request progress.
        /// </summary>
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
        /// Starts the load: issues an asynchronous editor scene load when in play mode;
        /// logs an error and aborts when running outside the editor.
        /// </summary>
        protected override void OnStart()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                return;
            }
            var param = new LoadSceneParameters { loadSceneMode = LoadSceneMode };
            _request = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(AssetPath, param);
#else
            Facade.GetSystem<LogSystem>().LogError(
                $"{nameof(AssetDatabaseSceneLoaderAsync)} is not supported in runtime mode.");
            Abort();
#endif
        }

        /// <summary>No cleanup is required; editor scene loads stop on their own.</summary>
        protected override void OnStop()
        {

        }

        /// <summary>
        /// Waits until the play-mode request completes, then captures the scene instance;
        /// captures it immediately via a synchronous open in edit mode.
        /// </summary>
        protected override void OnUpdate()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                ObtainSceneObject();
                return;
            }
            if (null == _request || !_request.isDone) {
                return;
            }
            ObtainSceneObject();
#endif
        }

        /// <summary>Force completion is not supported for async scene loads.</summary>
        /// <exception cref="BundleNotSupportedException">Always thrown.</exception>
        protected override void OnForceComplete()
        {
            throw new BundleNotSupportedException("Force complete async scene loader is not supported.");
        }

        /// <summary>
        /// Captures the scene instance and finishes the loader; logs an error and aborts on failure.
        /// Opens the scene synchronously in edit mode (single or additive to match the configured
        /// load mode) and looks it up by asset path in play mode.
        /// </summary>
        private void ObtainSceneObject()
        {
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

        /// <summary>
        /// Gets the loaded scene.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader has not finished.</exception>
        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }

        /// <summary>Returns a diagnostic string with the loader type, asset path, load mode, state, and progress.</summary>
        /// <returns>A human-readable description of this loader.</returns>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, AssetPath: {AssetPath}, LoadSceneMode: {LoadSceneMode}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}