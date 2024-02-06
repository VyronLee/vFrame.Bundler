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

        Asset LoadAsset(string path, Type type);
        AssetAsync LoadAssetAsync(string path, Type type);
        Asset LoadAssetWithSubAssets(string path, Type type);
        AssetAsync LoadAssetWithSubAssetsAsync(string path, Type type);

        Asset<T> LoadAsset<T>(string path) where T : Object;
        AssetAsync<T> LoadAssetAsync<T>(string path) where T : Object;
        Asset<T> LoadAssetWithSubAssets<T>(string path) where T : Object;
        AssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object;

        Scene LoadScene(string path, LoadSceneMode mode);
        SceneAsync LoadSceneAsync(string path, LoadSceneMode mode);

        void Update();
        void Collect();
        void SetLogLevel(int level);
    }
}