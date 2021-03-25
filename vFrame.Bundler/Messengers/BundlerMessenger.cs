//------------------------------------------------------------
//        File:  BundlerMessenger.cs
//       Brief:  BundlerMessenger
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 11:22
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using vFrame.Bundler.Assets;
using vFrame.Bundler.Base.Pools;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Messengers
{
    public class BundlerMessenger : MonoBehaviour
    {
        public static readonly HashSet<BundlerMessenger> Messengers = new HashSet<BundlerMessenger>();

        private HashSet<AssetBase> _assets;
        private Dictionary<Type, AssetBase> _typedAssets;

        private GameObject _targetObject;
        private string _targetName;

        [SerializeField]
        private string _instanceId;

        public List<AssetBase> GetAssets() {
            var assets = new List<AssetBase>();
            if (null != _assets)
                assets.AddRange(_assets.ToList());

            if (null != _typedAssets)
                assets.AddRange(_typedAssets.Select(v => v.Value));

            return assets;
        }

        public bool Alive {
            get { return null != this; }
        }

        public string TargetName {
            get { return _targetName; }
        }

        private void Awake() {
            _targetObject = gameObject;
            _targetName = name;

            if (!string.IsNullOrEmpty(_instanceId)) {
                if (!RecoverRefs()) {
                    Logger.LogError("BundlerMessenger:Awake - Recover refs failed, see console output!");
                }
            }
            _instanceId = GetInstanceID().ToString();

            Logger.LogVerbose("BundlerMessenger::Awake: {0}", this);

            hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
        }

        private static BundlerMessenger FindMessengerInstance(string instanceId) {
            foreach (var bundlerMessenger in Messengers) {
                if (bundlerMessenger._instanceId == instanceId) {
                    return bundlerMessenger;
                }
            }
            return null;
        }

        private bool RecoverRefs() {
            Logger.LogVerbose("BundlerMessenger::RecoverRefs: {0}", this);

            var clonedFromMessenger = FindMessengerInstance(_instanceId);
            if (!clonedFromMessenger) {
                Logger.LogError("BundlerMessenger::RecoverRefs: Messenger not found, instance id: {0}", _instanceId);
                return false;
            }

            _assets = new HashSet<AssetBase>();
            if (null != clonedFromMessenger._assets) {
                foreach (var asset in clonedFromMessenger._assets) {
                    Logger.LogVerbose("BundlerMessenger::RecoverRefs: {0}, loader: {1}", this, asset.Loader);
                    asset.Retain();
                    Logger.LogVerbose("BundlerMessenger::RecoverRefs: {0}, loader: {1}, finished!", this, asset.Loader);
                    _assets.Add(asset);
                }
            }

            _typedAssets = new Dictionary<Type, AssetBase>();
            if (null != clonedFromMessenger._typedAssets) {
                foreach (var kv in clonedFromMessenger._typedAssets) {
                    Logger.LogVerbose("BundlerMessenger::RecoverRefs: {0}, loader: {1}", this, kv.Value.Loader);
                    kv.Value.Retain();
                    Logger.LogVerbose("BundlerMessenger::RecoverRef: {0}, loader: {1}, finished", this, kv.Value.Loader);
                    _typedAssets.Add(kv.Key, kv.Value);
                }
            }

            Logger.LogVerbose("BundlerMessenger::RecoverRef: {0}, finished!", this);

            Messengers.Add(this);
            return true;
        }

        private void OnDestroy() {
            Logger.LogVerbose("BundlerMessenger::OnDestroy: {0}", this);
            ReleaseRef();
        }

        public void ReleaseRef() {
            if (null != _assets) {
                foreach (var asset in _assets) {
                    Logger.LogVerbose("BundlerMessenger::ReleaseRef: {0}, loader: {1}", this, asset.Loader);
                    asset.Release();
                    Logger.LogVerbose("BundlerMessenger::ReleaseRef: {0}, loader: {1}, finished!", this, asset.Loader);
                }

                HashSetPool<AssetBase>.Return(_assets);
            }

            if (null != _typedAssets) {
                foreach (var kv in _typedAssets) {
                    Logger.LogVerbose("BundlerMessenger::ReleaseRef: {0}, loader: {1}", this, kv.Value.Loader);
                    kv.Value.Release();
                    Logger.LogVerbose("BundlerMessenger::ReleaseRef: {0}, loader: {1}, finished!", this, kv.Value.Loader);
                }

                DictionaryPool<Type, AssetBase>.Return(_typedAssets);
            }

            Messengers.Remove(this);
        }

        public void RetainRef(AssetBase asset) {
            if (null == _assets)
                _assets = HashSetPool<AssetBase>.Get();

            if (_assets.Contains(asset))
                return;
            _assets.Add(asset);

            Logger.LogVerbose("BundlerMessenger::RetainRef: {0}, loader: {1}", this, asset.Loader);

            asset.Retain();

            Logger.LogVerbose("BundlerMessenger::RetainRef: {0}, loader: {1}, finished!", this, asset.Loader);

            Messengers.Add(this);
        }

        public void RetainRef<TSetter>(AssetBase asset) {
            if (null == _typedAssets)
                _typedAssets = DictionaryPool<Type, AssetBase>.Get();

            var type = typeof(TSetter);
            if (_typedAssets.ContainsKey(type)) {
                Logger.LogVerbose("BundlerMessenger::RetainRef<T>: {0}, Setter exist: {1}, release previous asset: {2}",
                    this, type.FullName, _typedAssets[type].AssetPath);

                _typedAssets[type].Release();

                Logger.LogVerbose("BundlerMessenger::RetainRef<T>: {0}, Setter exist: {1}, release previous asset: {2}, finished!",
                    this, type.FullName, _typedAssets[type].AssetPath);
            }

            _typedAssets[type] = asset;

            Logger.LogVerbose("BundlerMessenger::RetainRef<T>: {0}, loader: {1}", this, asset.Loader);

            asset.Retain();

            Logger.LogVerbose("BundlerMessenger::RetainRef<T>: {0}, loader: {1}, finished!", this, asset.Loader);

            Messengers.Add(this);
        }

        public override string ToString() {
            return string.Format("[Messenger: {0}, TargetName: {1}, Target GameObject: {2}]",
                GetInstanceID(), TargetName, null != _targetObject ? _targetObject.GetInstanceID().ToString() : "(null)");
        }
    }
}