//------------------------------------------------------------
//        File:  IBundler.cs
//       Brief:  Bundler interface.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:05
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public interface IBundler
    {
        void Destroy();

        IAsset LoadAsset(string path, Type type);
        IAssetAsync LoadAssetAsync(string path, Type type);
        IAsset LoadAssetWithSubAssets(string path, Type type);
        IAssetAsync LoadAssetWithSubAssetsAsync(string path, Type type);

        IAsset<T> LoadAsset<T>(string path) where T : Object;
        IAssetAsync<T> LoadAssetAsync<T>(string path) where T : Object;
        IAsset<T> LoadAssetWithSubAssets<T>(string path) where T : Object;
        IAssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object;

        IScene LoadScene(string path, LoadSceneMode mode);
        ISceneAsync LoadSceneAsync(string path, LoadSceneMode mode);

        void Update();
        void Collect();
        void SetLogLevel(int level);
    }
}