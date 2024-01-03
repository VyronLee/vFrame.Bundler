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
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public interface IAsset<T> : IAsset where T : Object
    {
        new T GetRawAsset();
        new T Instantiate(Transform parent = null, bool stayWorldPosition = false);

        void SetTo<TComponent, TSetter>(TComponent target)
            where TComponent : Component
            where TSetter : PropertySetterProxy<TComponent, T>, new();
    }

    public interface IAsset
    {
        Object GetRawAsset();
        Object Instantiate(Transform parent = null, bool stayWorldPosition = false);

        void SetTo<TComponent, TObject, TSetter>(TComponent target)
            where TComponent : Component
            where TObject : Object
            where TSetter : PropertySetterProxy<TComponent, TObject>, new();
    }
}