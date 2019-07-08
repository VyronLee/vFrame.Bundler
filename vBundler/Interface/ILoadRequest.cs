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

namespace vBundler.Interface
{
    public interface ILoadRequest
    {
        IAsset GetAsset(Type type);
        IScene GetScene(LoadSceneMode mode);

        IAssetAsync GetAssetAsync(Type type);
        ISceneAsync GetSceneAsync(LoadSceneMode mode);
    }
}