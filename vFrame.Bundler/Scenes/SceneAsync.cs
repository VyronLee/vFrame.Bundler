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

namespace vFrame.Bundler.Scenes
{
    public class SceneAsync : SceneBase, ISceneAsync
    {
        public SceneAsync(string path, LoadSceneMode mode, BundleLoaderBase loader)
            : base(path, mode, loader) {
        }

        private AsyncOperation Operation { get; set; }

        public IEnumerator Await() {
            if (IsStarted) {
                while (!IsDone) {
                    yield return null;
                }
                yield break;
            }

            IsStarted = true;
            yield return Operation;
            IsDone = true;
        }

        public float Progress {
            get { return Operation != null ? Operation.progress : 0f; }
        }

        public bool IsStarted { get; private set; }

        public override bool IsDone { get; protected set; }

        protected override void LoadInternal() {
            Operation = SceneManager.LoadSceneAsync(_scenePath, _mode);
            if (Operation == null)
                throw new BundleSceneLoadFailedException("Could not load scene asynchronously: " + _path);

            Operation.allowSceneActivation = true;
        }
    }
}