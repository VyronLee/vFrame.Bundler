// ------------------------------------------------------------
//         File: Asset.cs
//        Brief: Asset.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 23:20
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public struct Asset : ILoaderHandler
    {
        Loader ILoaderHandler.Loader { get; set; }

        void ILoaderHandler.Update() {

        }

        public Object GetRawAsset() {
            return AssetHelper<Asset>.GetRawAsset(this);
        }

        public Object[] GetAllRawAssets() {
            return AssetHelper<Asset>.GetAllRawAssets(this);
        }

        public Object Instantiate(Transform parent = null, bool stayWorldPosition = false) {
            return AssetHelper<Asset>.Instantiate(this, parent, stayWorldPosition);
        }

        public void SetTo<TComponent, TObject, TSetter>(TComponent target)
            where TComponent : Component
            where TObject : Object
            where TSetter : PropertySetterProxy<TComponent, TObject>, new() {

            AssetHelper<Asset>.SetTo<TComponent, TObject, TSetter>(this, target);
        }

        public void Retain() {
            AssetHelper<Asset>.GetAssetLoader(this).Retain();
        }

        public void Release() {
            AssetHelper<Asset>.GetAssetLoader(this).Release();
        }
    }

    public struct Asset<T> : ILoaderHandler where T : Object
    {
        Loader ILoaderHandler.Loader { get; set; }

        void ILoaderHandler.Update() {

        }

        public T GetRawAsset() {
            return AssetHelper<Asset<T>>.GetRawAsset(this) as T;
        }

        public T[] GetAllRawAssets() {
            return AssetHelper<Asset<T>>.GetAllRawAssets(this) as T[];
        }

        public T Instantiate(Transform parent = null, bool stayWorldPosition = false) {
            return AssetHelper<Asset<T>>.Instantiate(this, parent, stayWorldPosition) as T;
        }

        public void SetTo<TComponent, TSetter>(TComponent target)
            where TComponent : Component
            where TSetter : PropertySetterProxy<TComponent, T>, new() {

            AssetHelper<Asset<T>>.SetTo<TComponent, T, TSetter>(this, target);
        }

        public void Retain() {
            AssetHelper<Asset<T>>.GetAssetLoader(this).Retain();
        }

        public void Release() {
            AssetHelper<Asset<T>>.GetAssetLoader(this).Release();
        }
    }
}