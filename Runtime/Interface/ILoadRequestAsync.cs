//------------------------------------------------------------
//        File:  ILoadRequestAsync.cs
//       Brief:  Async load request interface
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-14 19:57
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Interface
{
    public interface ILoadRequestAsync : ILoadRequest, IAsync
    {
        IAssetAsync GetAssetAsync<T>() where T : Object;
        IAssetAsync GetAssetAsync(Type type);
        ISceneAsync GetSceneAsync(LoadSceneMode mode);
    }
}