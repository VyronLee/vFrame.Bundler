// ------------------------------------------------------------
//         File: AssetDatabaseSceneLoader.cs
//        Brief: AssetDatabaseSceneLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 21:34
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace vFrame.Bundler
{
    internal class AssetDatabaseSceneLoader : SceneLoader
    {
        private UnityEngine.SceneManagement.Scene _sceneObject;
        private readonly int _startFrame;
        private readonly int _frameLength;

        public AssetDatabaseSceneLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

            _startFrame = Time.frameCount;
            _frameLength = Random.Range(
                bundlerContexts.Options.MinAsyncFrameCountWhenUsingAssetDatabase,
                bundlerContexts.Options.MaxAsyncFrameCountWhenUsingAssetDatabase);
        }

        public override float Progress => Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f);

        protected override void OnStart() {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                var mode = LoadSceneMode == LoadSceneMode.Single ? OpenSceneMode.Single : OpenSceneMode.Additive;
                _sceneObject = EditorSceneManager.OpenScene(AssetPath, mode);
                return;
            }
            var param = new LoadSceneParameters {loadSceneMode = LoadSceneMode};
            _sceneObject = EditorSceneManager.LoadSceneInPlayMode(AssetPath, param);
#else
            Facade.GetSystem<LogSystem>().LogError(
                $"{nameof(AssetDatabaseSceneLoader)} is not supported in runtime mode.");
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