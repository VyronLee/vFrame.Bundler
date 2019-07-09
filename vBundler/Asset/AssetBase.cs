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
using vBundler.Loader;
using vBundler.Messenger;
using Object = UnityEngine.Object;

namespace vBundler.Asset
{
    public abstract class AssetBase : IAsset
    {
        protected readonly string _path;
        protected readonly BundleLoaderBase _target;
        protected readonly Type _type;
        protected Object _asset;

        protected AssetBase(string path, Type type, BundleLoaderBase target)
        {
            _target = target;
            _path = path;
            _type = type;

            if (target != null && !target.IsDone)
                throw new InvalidProgramException("Loader hasn't finished: " + target.AssetBundlePath);

            LoadAssetInternal();
        }

        public virtual bool IsDone { get; set; }

        public void Retain()
        {
            if (_target != null)
                _target.Retain();
        }

        public void Release()
        {
            if (_target != null)
                _target.Release();
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
            // Unsubscribe current node
            UnsubscribeDestroyedMessenger(gameObject);

            // Unsubscribe children nodes
            var messengers = gameObject.GetComponentsInChildren<DestroyedMessenger>(true);
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

        protected abstract void LoadAssetInternal();

        private void SubscribeDestroyedMessenger(GameObject gameObject)
        {
            var messenger = gameObject.GetComponent<DestroyedMessenger>();
            if (!messenger)
                messenger = gameObject.AddComponent<DestroyedMessenger>();

            messenger.RetainRef(this);
        }

        private void UnsubscribeDestroyedMessenger(GameObject gameObject)
        {
            var messenger = gameObject.GetComponent<DestroyedMessenger>();
            if (!messenger)
                return;

            messenger.ReleaseRef();
        }
    }
}