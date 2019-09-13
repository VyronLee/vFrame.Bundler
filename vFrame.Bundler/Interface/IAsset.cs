//------------------------------------------------------------
//        File:  IAsset.cs
//       Brief:  Asset interface.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:04
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using UnityEngine;

namespace vFrame.Bundler.Interface
{
    public interface IAsset
    {
        Object GetAsset();
        GameObject InstantiateGameObject();
        void DestroyGameObject(GameObject gameObject);

        void SetTo(Component target, string propName);

        void Retain();
        void Release();
    }
}