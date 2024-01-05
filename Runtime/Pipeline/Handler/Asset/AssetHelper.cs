// ------------------------------------------------------------
//         File: AssetHelper.cs
//        Brief: AssetHelper.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 13:23
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal static class AssetHelper<T> where T: ILoaderHandler
    {
        public static AssetLoader GetAssetLoader(T loaderHandler) {
            return loaderHandler.Loader as AssetLoader
                   ?? throw new ArgumentException("AssetLoader expected, got: "
                       + (loaderHandler.Loader?.GetType().Name ?? "null"));
        }

        public static Bundler GetFacade(T loaderHandler) {
            return loaderHandler.BundlerContexts.Bundler;
        }

        public static Object GetRawAsset(T loaderHandler) {
            return GetAssetLoader(loaderHandler).AssetObject;
        }

        public static Object[] GetAllRawAssets(T loaderHandler) {
            return GetAssetLoader(loaderHandler).AssetObjects;
        }

        public static Object Instantiate(T loaderHandler, Transform parent = null, bool stayWorldPosition = false) {
            var ret = Object.Instantiate(GetAssetLoader(loaderHandler).AssetObject, parent, stayWorldPosition);
            var proxySystem = GetFacade(loaderHandler).GetSystem<LinkSystem>();
            proxySystem.LinkObject(ret, loaderHandler);
            return ret;
        }

        public static void SetTo<TComponent, TObject, TProxy>(T loaderHandler, TComponent target)
            where TComponent : Component
            where TObject : Object
            where TProxy : PropertySetterProxy<TComponent, TObject>, new() {

            var proxySystem = GetFacade(loaderHandler).GetSystem<LinkSystem>();
            proxySystem.RebindProxy<TComponent, TObject, TProxy>(loaderHandler, target);
        }
    }
}