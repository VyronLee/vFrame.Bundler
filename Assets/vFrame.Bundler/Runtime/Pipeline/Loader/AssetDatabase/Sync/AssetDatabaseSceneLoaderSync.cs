// ------------------------------------------------------------
//         File: AssetDatabaseSceneLoaderSync.cs
//        Brief: Synchronous editor scene loader: opens the scene via OpenScene in edit mode or
//               LoadSceneInPlayMode in play mode; aborts with an error log in runtime builds.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:44:18
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    /// Synchronous scene loader backed by the Unity editor's asset database.
    /// Opens the scene immediately via <see cref="UnityEditor.SceneManagement.EditorSceneManager.OpenScene"/> in edit
    /// mode or <see cref="UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode"/> in play mode;
    /// aborts with an error log in runtime (non-editor) builds.
    /// </summary>
    internal class AssetDatabaseSceneLoaderSync : SceneLoader
    {
        /// <summary>Scene handle captured when the editor opens or loads the scene.</summary>
        private UnityEngine.SceneManagement.Scene _sceneObject;

        /// <summary>
        /// Initializes the loader with the bundler-wide and per-request loader contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler contexts (systems, search paths, etc.).</param>
        /// <param name="loaderContexts">Per-loader contexts supplying <see cref="SceneLoader.AssetPath"/> and scene mode.</param>
        public AssetDatabaseSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {
        }

        /// <summary>
        /// Reports binary progress: 1 once loading has finished, otherwise 0 (loading is instantaneous in editor).
        /// </summary>
        /// <returns>1f when done; otherwise 0f.</returns>
        [JsonSerializableProperty]
        public override float Progress => IsDone ? 1f : 0f;

        /// <summary>
        /// Opens or loads the scene synchronously through the editor scene manager and marks the task finished,
        /// or aborts the task with an error log when running outside the editor.
        /// </summary>
        protected override void OnStart()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                var mode = LoadSceneMode == LoadSceneMode.Single
                    ? UnityEditor.SceneManagement.OpenSceneMode.Single
                    : UnityEditor.SceneManagement.OpenSceneMode.Additive;
                _sceneObject = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(AssetPath, mode);
                return;
            }
            var param = new LoadSceneParameters { loadSceneMode = LoadSceneMode };
            _sceneObject = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(AssetPath, param);
            Finish();
#else
            Facade.GetSystem<LogSystem>().LogError(
                $"{nameof(AssetDatabaseSceneLoaderSync)} is not supported in runtime mode.");
            Abort();
#endif
        }

        /// <summary>Performs no cleanup; the opened scene is left as-is when the loader stops.</summary>
        protected override void OnStop()
        {

        }

        /// <summary>Marks the task finished; the scene is already open by <see cref="OnStart"/>.</summary>
        protected override void OnUpdate()
        {
            Finish();
        }

        /// <summary>Marks the task finished; the scene is already open by <see cref="OnStart"/>.</summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>
        /// Gets the scene opened by this loader.
        /// </summary>
        /// <returns>The <see cref="UnityEngine.SceneManagement.Scene"/> captured during loading.</returns>
        /// <exception cref="BundleAssetNotReadyException">Thrown when the loader has not finished yet.</exception>
        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }

        /// <summary>
        /// Returns a string describing the loader's type, target asset path, scene mode, task state, and progress.
        /// </summary>
        /// <returns>A human-readable summary of the loader state.</returns>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, AssetPath: {AssetPath}, LoadSceneMode: {LoadSceneMode}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}