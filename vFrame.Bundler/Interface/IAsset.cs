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
using vFrame.Bundler.Base;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Interface
{
    public interface IAsset
    {
        Object GetAsset();
        GameObject InstantiateGameObject();
        void DestroyGameObject(GameObject gameObject);
        void SetTo<T1, T2, TSetter>(T1 target)
            where T1 : Component
            where T2 : Object
            where TSetter : PropertySetterProxy<T1, T2>, new();
        void Retain();
        void Release();
    }
}