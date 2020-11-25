//------------------------------------------------------------
//        File:  ILoadRequest.cs
//       Brief:  Load request interface
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-14 15:31
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Interface
{
    public interface ILoadRequest : IReference
    {
        IAsset GetAsset<T>() where T : Object;
        IAsset GetAsset(Type type);
        IScene GetScene(LoadSceneMode mode);

        IAssetAsync GetAssetAsync<T>() where T : Object;
        IAssetAsync GetAssetAsync(Type type);
        ISceneAsync GetSceneAsync(LoadSceneMode mode);
    }
}