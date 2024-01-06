// ------------------------------------------------------------
//         File: AssetDatabaseSceneLoaderSync.cs
//        Brief: AssetDatabaseSceneLoaderSync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 21:34
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    internal class AssetDatabaseSceneLoaderSync : SceneLoader
    {
        private UnityEngine.SceneManagement.Scene _sceneObject;

        public AssetDatabaseSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {
        }

        public override float Progress => IsDone ? 1f : 0f;

        protected override void OnStart() {
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

        protected override void OnStop() {

        }

        protected override void OnUpdate() {
            Finish();
        }

        protected override void OnForceComplete() {
            Finish();
        }

        public override UnityEngine.SceneManagement.Scene SceneObject {
            get {
                ThrowIfNotFinished();
                return _sceneObject;
            }
        }
    }
}