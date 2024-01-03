// ------------------------------------------------------------
//         File: Asset.cs
//        Brief: Asset.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 23:20
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public class Asset : LoaderHandler, IAsset
    {
        internal AssetLoader AssetLoader { get; private set; }

        internal override Loader Loader {
            set => AssetLoader = value as AssetLoader
                                 ?? throw new ArgumentException($"AssetLoader expected, got: {value.GetType()}");
        }

        public Object GetRawAsset() {
            return AssetLoader?.AssetObject;
        }

        public Object Instantiate(Transform parent = null, bool stayWorldPosition = false) {
            if (null == AssetLoader) {
                return null;
            }
            var ret = Object.Instantiate(AssetLoader.AssetObject, parent, stayWorldPosition);
            // TODO: Retain object
            return ret;
        }

        public void SetTo<TComponent, TObject, TSetter>(TComponent target)
            where TComponent : Component
            where TObject : Object
            where TSetter : PropertySetterProxy<TComponent, TObject>, new() {

            var setter = ObjectPool<TSetter>.Get();
            setter.Set(target, GetRawAsset() as TObject);
            ObjectPool<TSetter>.Return(setter);
        }
    }

    public class Asset<T> : Asset, IAsset<T> where T : Object
    {
        public new T GetRawAsset() {
            return base.GetRawAsset() as T;
        }

        public new T Instantiate(Transform parent = null, bool stayWorldPosition = false) {
            return base.Instantiate(parent, stayWorldPosition) as T;
        }

        public void SetTo<TComponent, TSetter>(TComponent target)
            where TComponent : Component
            where TSetter : PropertySetterProxy<TComponent, T>, new() {

            base.SetTo<TComponent, T, TSetter>(target);
        }
    }
}