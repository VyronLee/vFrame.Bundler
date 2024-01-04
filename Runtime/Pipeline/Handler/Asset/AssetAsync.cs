// ------------------------------------------------------------
//         File: AssetAsync.cs
//        Brief: AssetAsync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 19:39
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public struct AssetAsync : ILoaderHandler
    {
        Loader ILoaderHandler.Loader { get; set; }

        void ILoaderHandler.Update() {

        }

        public Object GetRawAsset() {
            return AssetHelper<AssetAsync>.GetRawAsset(this);
        }

        public Object[] GetAllRawAssets() {
            return AssetHelper<AssetAsync>.GetAllRawAssets(this);
        }

        public Object Instantiate(Transform parent = null, bool stayWorldPosition = false) {
            return AssetHelper<AssetAsync>.Instantiate(this, parent, stayWorldPosition);
        }

        public void SetTo<TComponent, TObject, TSetter>(TComponent target)
            where TComponent : Component
            where TObject : Object
            where TSetter : PropertySetterProxy<TComponent, TObject>, new() {

            AssetHelper<AssetAsync>.SetTo<TComponent, TObject, TSetter>(this, target);
        }

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current => null;

        public bool IsDone => AssetHelper<AssetAsync>.GetAssetLoader(this).IsDone;

        public float Progress => AssetHelper<AssetAsync>.GetAssetLoader(this).Progress;
    }

    public struct AssetAsync<T> : ILoaderHandler where T : Object
    {
        Loader ILoaderHandler.Loader { get; set; }

        void ILoaderHandler.Update() {

        }

        public T GetRawAsset() {
            return AssetHelper<AssetAsync<T>>.GetRawAsset(this) as T;
        }

        public T[] GetAllRawAssets() {
            return AssetHelper<AssetAsync<T>>.GetAllRawAssets(this) as T[];
        }

        public T Instantiate(Transform parent = null, bool stayWorldPosition = false) {
            return AssetHelper<AssetAsync<T>>.Instantiate(this, parent, stayWorldPosition) as T;
        }

        public void SetTo<TComponent, TSetter>(TComponent target)
            where TComponent : Component
            where TSetter : PropertySetterProxy<TComponent, T>, new() {

            AssetHelper<AssetAsync<T>>.SetTo<TComponent, T, TSetter>(this, target);
        }

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current => null;

        public bool IsDone => AssetHelper<AssetAsync<T>>.GetAssetLoader(this).IsDone;

        public float Progress => AssetHelper<AssetAsync<T>>.GetAssetLoader(this).Progress;
    }
}