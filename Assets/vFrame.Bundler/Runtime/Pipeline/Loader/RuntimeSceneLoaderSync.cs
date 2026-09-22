// ------------------------------------------------------------
//         File: RuntimeSceneLoaderSync.cs
//        Brief: Loads a scene in a single synchronous step and exposes the resulting Scene handle.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:56:03
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    /// Loads a scene synchronously via <see cref="SceneManager.LoadScene"/> and completes on the
    /// same frame it starts, since the operation yields no incremental progress.
    /// </summary>
    internal class RuntimeSceneLoaderSync : SceneLoader
    {
        /// <summary>
        /// Scene handle resolved by path right after the synchronous load; invalid if the load failed.
        /// </summary>
        private UnityEngine.SceneManagement.Scene _sceneObject;

        /// <summary>
        /// Initializes the loader with the bundler-wide and per-request contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler state and systems.</param>
        /// <param name="loaderContexts">Request state, including the scene asset path and load mode.</param>
        protected RuntimeSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>
        /// Load progress; a synchronous load is binary, so this is 1 once done and 0 before that.
        /// </summary>
        [JsonSerializableProperty]
        public override float Progress => IsDone ? 1f : 0f;

        /// <summary>
        /// Loads the scene synchronously and finishes immediately if the resulting scene is valid;
        /// otherwise logs an error and aborts.
        /// </summary>
        protected override void OnStart()
        {
            SceneManager.LoadScene(AssetPath, LoadSceneMode);
            _sceneObject = SceneManager.GetSceneByPath(AssetPath);
            if (null != _sceneObject && _sceneObject.IsValid()) {
                Finish();
                return;
            }
            Facade.GetSystem<LogSystem>().LogError("Get scene instance failed: {0}", AssetPath);
            Abort();
        }

        /// <summary>
        /// No cleanup is required for a synchronous scene load.
        /// </summary>
        protected override void OnStop()
        {

        }

        /// <summary>
        /// Marks the load finished; the scene is already available by the time updates run.
        /// </summary>
        protected override void OnUpdate()
        {
            Finish();
        }

        /// <summary>
        /// Marks the load finished when force-completed externally.
        /// </summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>
        /// Gets the loaded scene handle.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">The loader has not finished.</exception>
        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }
    }
}