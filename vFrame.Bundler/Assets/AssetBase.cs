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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using vFrame.Bundler.Base;
using vFrame.Bundler.Base.Pools;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Extension;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Messengers;
using vFrame.Bundler.Utils;
using Logger = vFrame.Bundler.Logs.Logger;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Assets
{
    public abstract class AssetBase : IAsset
    {
        internal readonly BundlerContext _context;
        protected readonly string _path;
        protected readonly BundleLoaderBase _loader;
        protected readonly Type _type;

        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertiesCache
            = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private static readonly Dictionary<string, string> _assetNameCache = new Dictionary<string, string>(2048);

        public string AssetPath {
            get { return _path; }
        }

        public BundleLoaderBase Loader {
            get { return _loader; }
        }

        public string LoaderPath {
            get { return null != _loader ? _loader.AssetBundlePath : "null"; }
        }

        internal AssetBase(string path, Type type, BundleLoaderBase target, BundlerContext context) {
            _loader = target;
            _path = path;
            _type = type;
            _context = context;

            ThrowIfBundleError();

            LoadAsset();
        }

        private void ThrowIfBundleError() {
            if (null == _loader) {
                return;
            }

            var errors = ListPool<BundleLoaderBase>.Get();
            if (_loader.Validate(ref errors)) {
                ListPool<BundleLoaderBase>.Return(errors);
                return;
            }

            try {
                throw new BundleException(string.Format(
                    "One or more bundles and it's dependencies in an invalid state:\n  this loader: \n    {0}\n  errors: \n    {1}",
                    _loader,
                    string.Join("\n    ", errors.ConvertAll(v => v.ToString()).ToArray())));
            }
            finally {
                ListPool<BundleLoaderBase>.Return(errors);
            }
        }

        private void LoadAsset() {
            LoadAssetInternal();
        }

        protected abstract void LoadAssetInternal();

        public bool IsDone { get; set; }

        public void Retain() {
            if (_loader != null)
                _loader.Retain();
        }

        public void Release() {
            if (_loader != null)
                _loader.Release();
        }

        protected string GetAssetName() {
            string assetName;
            if (!_assetNameCache.TryGetValue(_path, out assetName))
                assetName = _assetNameCache[_path] = PathUtility.GetAssetName(_path);
            return assetName;
        }

        public abstract Object GetAsset();

        public GameObject InstantiateGameObject() {
            if (!IsDone)
                throw new BundleAssetNotReadyException(
                    string.Format("Asset not ready, path: {0}, loader: {1}", _path, _loader));

            var prefab = GetAsset() as GameObject;
            if (!prefab)
                throw new BundleAssetTypeNotMatchException(
                    string.Format("Asset not typeof GameObject, path: {0}, loader: {1}", _path, _loader));

            var go = Object.Instantiate(prefab);

            Logger.LogVerbose("Instantiate gameObject: {0}, from bundle: {1}", AssetPath, LoaderPath);

            SubscribeDestroyedMessenger(go);

            return go;
        }

        public void DestroyGameObject(GameObject gameObject) {
            // Unsubscribe parent node
            UnsubscribeDestroyedMessenger(gameObject);

            // Unsubscribe children nodes
            var messengers = gameObject.GetComponentsInChildren<BundlerMessenger>(true);
            foreach (var messenger in messengers)
                UnsubscribeDestroyedMessenger(messenger.gameObject);

            gameObject.DestroyEx();
        }

        public void SetTo<T1, T2, TSetter>(T1 target)
            where T1 : Component
            where T2 : Object
            where TSetter : PropertySetterProxy<T1, T2>, new() {
            if (!IsDone)
                throw new BundleAssetNotReadyException(
                    string.Format("Asset not ready: {0}, loader: {1}", _path, _loader));

            var setter = ObjectPool<TSetter>.Get();
            setter.Set(target, GetAsset() as T2);
            ObjectPool<TSetter>.Return(setter);

            SubscribeDestroyedMessenger<TSetter>(target.gameObject);
        }

        private static PropertyInfo GetProperty(Component target, string propertyName) {
            Dictionary<string, PropertyInfo> propertyDict;
            var typeInfo = target.GetType();
            if (!_propertiesCache.TryGetValue(typeInfo, out propertyDict))
                propertyDict = _propertiesCache[typeInfo] = new Dictionary<string, PropertyInfo>();

            PropertyInfo propertyInfo;
            if (!propertyDict.TryGetValue(propertyName, out propertyInfo))
                propertyInfo = propertyDict[propertyName] = target.GetType().GetProperty(propertyName);
            return propertyInfo;
        }

        private void SubscribeDestroyedMessenger(GameObject gameObject) {
            var messenger = gameObject.GetComponent<BundlerMessenger>();
            if (!messenger)
                messenger = gameObject.AddComponent<BundlerMessenger>();

            messenger.RetainRef(this);
        }

        private void SubscribeDestroyedMessenger<TSetter>(GameObject gameObject) {
            var messenger = gameObject.GetComponent<BundlerMessenger>();
            if (!messenger)
                messenger = gameObject.AddComponent<BundlerMessenger>();

            messenger.RetainRef<TSetter>(this);
        }

        private void UnsubscribeDestroyedMessenger(GameObject gameObject) {
            var messenger = gameObject.GetComponent<BundlerMessenger>();
            if (!messenger)
                return;

            messenger.ReleaseRef();
        }

        public virtual void Dispose() {
        }

        public override string ToString() {
            return string.Format("[{0} path: {1}, type: {2}, done: {3}, loader: {4}]",
                GetType().Name,
                _path,
                _type,
                IsDone,
                _loader
            );
        }
    }
}