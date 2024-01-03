// ------------------------------------------------------------
//         File: RuntimeSceneLoaderSync.cs
//        Brief: RuntimeSceneLoaderSync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 23:32
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    internal class RuntimeSceneLoaderSync : SceneLoader
    {
        private UnityEngine.SceneManagement.Scene _sceneObject;

        protected RuntimeSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

        }

        public override float Progress => IsDone ? 1f : 0f;

        protected override void OnStart() {
            SceneManager.LoadScene(AssetPath, LoadSceneMode);
            _sceneObject = SceneManager.GetSceneByPath(AssetPath);
            if (null != _sceneObject && _sceneObject.IsValid()) {
                Finish();
                return;
            }
            Facade.GetSystem<LogSystem>().LogError("Get scene instance failed: {0}", AssetPath);
            Abort();
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