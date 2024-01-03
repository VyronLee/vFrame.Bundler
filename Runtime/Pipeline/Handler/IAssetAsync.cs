//------------------------------------------------------------
//        File:  IAssetAsync.cs
//       Brief:  IAssetAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:05
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public interface IAssetAsync<T> : IAsset<T>, IAsync where T : Object
    {
    }

    public interface IAssetAsync : IAsset, IAsync
    {
    }
}