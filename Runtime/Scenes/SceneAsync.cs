//------------------------------------------------------------
//        File:  SceneAsync.cs
//       Brief:  SceneAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:14
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Utils;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace vFrame.Bundler.Scenes
{
    public class SceneAsync : SceneBase, ISceneAsync, IAsyncProcessor
    {
        internal SceneAsync(string path, LoadSceneMode mode, BundlerContext context, BundleLoaderBase loader)
            : base(path, mode, context, loader) {
        }

        private SceneAsyncRequest Request { get; set; }

        public override void Dispose() {
            if (null != Context && null != Context.CoroutinePool) {
                if (null != Request) {
                    AsyncRequestHelper.Uninstall(Context.CoroutinePool, Request);
                }
                AsyncRequestHelper.Uninstall(Context.CoroutinePool, this);
            }
            base.Dispose();
        }

        public IEnumerator OnAsyncProcess() {
            IsStarted = true;
            yield return Request;
            Scene = Request.Scene;
            IsDone = true;
        }

        public float Progress {
            get { return Request != null ? Request.Progress : 0f; }
        }

        public bool IsStarted { get; private set; }

        protected override void LoadInternal() {
#if UNITY_EDITOR
            Request = new EditorSceneAsync(Context);
#else
            Request = new RuntimeSceneAsync();
#endif
            Request.Load(_path, _mode);

            AsyncRequestHelper.Setup(Context.CoroutinePool, Request);
            AsyncRequestHelper.Setup(Context.CoroutinePool, this);
        }

        protected override IEnumerator OnUnload() {
            yield return Request.Unload();
        }

#if UNITY_EDITOR
        protected override void OnUnloadInEditMode() {
            if (!EditorApplication.isPlaying) {
                EditorSceneManager.CloseScene(Scene, true);
            }
        }
#endif

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {
        }

        public object Current { get; private set; }
        public bool IsSetup { get; set; }
        public int ThreadHandle { get; set; }

        private abstract class SceneAsyncRequest : IAsync, IAsyncProcessor
        {
            public abstract bool IsStarted { get; protected set; }
            public bool IsDone { get; protected set; }
            public abstract float Progress { get; }
            public Scene Scene { get; protected set; }
            public abstract void Load(string scenePath, LoadSceneMode mode);
            public abstract IEnumerator Unload();
            public bool MoveNext() {
                return !IsDone;
            }

            public void Reset() {
            }

            public object Current { get; private set; }
            public bool IsSetup { get; set; }
            public int ThreadHandle { get; set; }
            public abstract IEnumerator OnAsyncProcess();
        }

        private class RuntimeSceneAsync : SceneAsyncRequest
        {
            private AsyncOperation _request;
            private string _path;

            public override float Progress {
                get {
                    return null == _request ? 0f : _request.progress;
                }
            }

            public override IEnumerator OnAsyncProcess() {
                if (null == _request) {
                    yield break;
                }

                IsStarted = true;
                yield return _request;

                Scene = SceneManager.GetSceneByPath(_path);
                if (Scene == null)
                    throw new BundleInstanceNotFoundException("No such scene instance: " + _path);

                IsDone = true;
            }

            public override void Load(string scenePath, LoadSceneMode mode) {
                _path = scenePath;
                _request = SceneManager.LoadSceneAsync(scenePath, mode);
                if (_request == null)
                    throw new BundleSceneLoadFailedException("Could not load scene asynchronously: " + scenePath);

                _request.allowSceneActivation = true;
            }

            public override IEnumerator Unload() {
                yield return SceneManager.UnloadSceneAsync(_path);
            }

            public override bool IsStarted { get; protected set; }
        }

#if UNITY_EDITOR
        private class EditorSceneAsync : SceneAsyncRequest
        {
            private int _startFrame;
            private int _frameLength;

            private string _path;
            private LoadSceneMode _mode;
            private AsyncOperation _request;
            private readonly BundlerContext _context;

            internal EditorSceneAsync(BundlerContext context) {
                _context = context;
            }

            public override IEnumerator OnAsyncProcess() {
                IsStarted = true;
                while (Time.frameCount - _startFrame < _frameLength) {
                    yield return null;
                }
                yield return LoadInternal();
                IsDone = true;
            }

            public override float Progress {
                get { return Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f); }
            }

            public override void Load(string scenePath, LoadSceneMode mode) {
                _path = scenePath;
                _mode = mode;
                _startFrame = Time.frameCount;
                _frameLength = Random.Range(
                    _context.Options.MinAsyncFrameCountWhenUsingAssetDatabase,
                    _context.Options.MaxAsyncFrameCountWhenUsingAssetDatabase);
            }

            public override IEnumerator Unload() {
                if (!EditorApplication.isPlaying) {
                    EditorSceneManager.CloseScene(Scene, true);
                    yield break;
                }
                yield return SceneManager.UnloadSceneAsync(_path);
            }

            private IEnumerator LoadInternal() {
                if (!EditorApplication.isPlaying) {
                    var realPath = PathUtility.RelativeDataPathToRelativeProjectPath(_path) + ".unity";
                    Scene = EditorSceneManager.OpenScene(realPath, _mode == LoadSceneMode.Single
                        ? OpenSceneMode.Single
                        : OpenSceneMode.Additive);
                    yield break;
                }

                var param = new LoadSceneParameters {loadSceneMode = _mode};
                _request = EditorSceneManager.LoadSceneAsyncInPlayMode(_path, param);
                _request.allowSceneActivation = true;
                yield return _request;
                Scene = SceneManager.GetSceneByPath(_path);
            }

            public override bool IsStarted { get; protected set; }
        }
#endif
    }
}