//------------------------------------------------------------
//        File:  SceneSync.cs
//       Brief:  SceneSync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:15
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Loaders;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using vFrame.Bundler.Utils;
#endif

namespace vFrame.Bundler.Scenes
{
    public class SceneSync : SceneBase
    {
        public SceneSync(string path, LoadSceneMode mode, BundlerOptions options, BundleLoaderBase loader)
            : base(path, mode, options, loader) {
        }

        protected override void LoadInternal() {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                var mode = _mode == LoadSceneMode.Single ? OpenSceneMode.Single : OpenSceneMode.Additive;
                var realPath = PathUtility.RelativeDataPathToRelativeProjectPath(_scenePath) + ".unity";
                Scene = EditorSceneManager.OpenScene(realPath, mode);
                return;
            }
            var param = new LoadSceneParameters {loadSceneMode = _mode};
            EditorSceneManager.LoadSceneInPlayMode(_path, param);
#else
            SceneManager.LoadScene(_scenePath, _mode);
#endif
            Scene = SceneManager.GetSceneByPath(_scenePath);
        }

        protected override IEnumerator OnUnload() {
            yield return SceneManager.UnloadSceneAsync(_scenePath);
        }

#if UNITY_EDITOR
        protected override void OnUnloadInEditMode() {
            if (!EditorApplication.isPlaying) {
                EditorSceneManager.CloseScene(Scene, true);
            }
        }
#endif

    }
}