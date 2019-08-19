//------------------------------------------------------------
//        File:  AssetBase.cs
//       Brief:  Assets loader base class.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:00
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using UnityEngine;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Loaders;
using vBundler.Messengers;
using Object = UnityEngine.Object;

namespace vBundler.Assets
{
    public abstract class AssetBase : IAsset
    {
        protected readonly string _path;
        protected readonly BundleLoaderBase _loader;
        protected readonly Type _type;
        protected Object _asset;

        protected AssetBase(string path, Type type, BundleLoaderBase target)
        {
            _loader = target;
            _path = path;
            _type = type;

            if (target != null && !target.IsDone)
                throw new BundleException("Loader hasn't finished: " + target.AssetBundlePath);

            LoadAsset();
        }

        private void LoadAsset()
        {
            LoadAssetInternal();
        }

        protected abstract void LoadAssetInternal();

        public virtual bool IsDone { get; set; }

        public void Retain()
        {
            if (_loader != null)
                _loader.Retain();
        }

        public void Release()
        {
            if (_loader != null)
                _loader.Release();
        }

        public virtual Object GetAsset()
        {
            if (!_asset)
                throw new BundleAssetNotReadyException("Asset has not loaded, path: " + _path);
            return _asset;
        }

        public GameObject InstantiateGameObject()
        {
            if (!IsDone)
                throw new BundleAssetNotReadyException("Asset not ready: " + _path);

            var prefab = GetAsset() as GameObject;
            if (!prefab)
                throw new BundleAssetTypeNotMatchException("Asset not typeof GameObject");

            var go = Object.Instantiate(prefab);

            SubscribeDestroyedMessenger(go);

            return go;
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            // Unsubscribe parent node
            UnsubscribeDestroyedMessenger(gameObject);

            // Unsubscribe children nodes
            var messengers = gameObject.GetComponentsInChildren<BundlerMessenger>(true);
            foreach (var messenger in messengers)
                UnsubscribeDestroyedMessenger(messenger.gameObject);

            Object.Destroy(gameObject);
        }

        public void SetTo(Component target, string propName)
        {
            if (!IsDone)
                throw new BundleAssetNotReadyException("Asset not ready: " + _path);

            var property = target.GetType().GetProperty(propName);
            if (property != null)
                property.SetValue(target, GetAsset(), null);
            else
                throw new ArgumentOutOfRangeException("Unknown property: " + propName);

            SubscribeDestroyedMessenger(target.gameObject);
        }

        private void SubscribeDestroyedMessenger(GameObject gameObject)
        {
            var messenger = gameObject.GetComponent<BundlerMessenger>();
            if (!messenger)
                messenger = gameObject.AddComponent<BundlerMessenger>();

            messenger.RetainRef(this);
        }

        private void UnsubscribeDestroyedMessenger(GameObject gameObject)
        {
            var messenger = gameObject.GetComponent<BundlerMessenger>();
            if (!messenger)
                return;

            messenger.ReleaseRef();
        }
    }
}
