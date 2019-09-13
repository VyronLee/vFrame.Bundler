//------------------------------------------------------------
//        File:  SceneAsync.cs
//       Brief:  SceneAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:14
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;

namespace vFrame.Bundler.Scenes
{
    public class SceneAsync : SceneBase, ISceneAsync
    {
        private bool _finished;

        public SceneAsync(string path, LoadSceneMode mode, BundleLoaderBase loader)
            : base(path, mode, loader)
        {
        }

        private AsyncOperation Operation { get; set; }

        public bool MoveNext()
        {
            if (_finished)
                return false;

            if (Operation == null || !Operation.isDone)
                return true;

            _finished = true;
            return false;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        public float Progress
        {
            get { return Operation != null ? Operation.progress : 0f; }
        }

        public override bool IsDone
        {
            get { return !MoveNext(); }
        }

        protected override void LoadInternal()
        {
            Operation = SceneManager.LoadSceneAsync(_scenePath, _mode);
            if (Operation == null)
                throw new BundleSceneLoadFailedException("Could not load scene asynchronously: " + _path);

            _finished = false;
            Operation.allowSceneActivation = true;
        }
    }
}