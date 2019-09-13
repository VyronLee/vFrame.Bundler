//------------------------------------------------------------
//        File:  SceneSync.cs
//       Brief:  SceneSync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:15
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using UnityEngine.SceneManagement;
using vFrame.Bundler.Loaders;

namespace vFrame.Bundler.Scenes
{
    public class SceneSync : SceneBase
    {
        public SceneSync(string path, LoadSceneMode mode, BundleLoaderBase loader) : base(path, mode, loader)
        {
        }

        protected override void LoadInternal()
        {
            SceneManager.LoadScene(_scenePath, _mode);
        }
    }
}