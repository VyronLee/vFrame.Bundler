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
using vFrame.Bundler.Utils.Pools;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler.Messengers
{
    public class BundlerMessenger : MonoBehaviour
    {
        public static readonly HashSet<BundlerMessenger> Messengers = new HashSet<BundlerMessenger>();

        private HashSet<AssetBase> _assets;
        private Dictionary<Type, AssetBase> _typedAssets;

        private bool _recovered;

        public List<AssetBase> GetAssets() {
            var assets = new List<AssetBase>();
            if (null != _assets) {
                assets.AddRange(_assets.ToList());
            }
            if (null != _typedAssets) {
                assets.AddRange(_typedAssets.Select(v => v.Value));
            }
            return assets;
        }

        private void Awake()
        {
            // Reference should be calculated when cloning from other instance
            RecoverRefs();

            hideFlags = HideFlags.HideAndDontSave;
        }

        private void RecoverRefs()
        {
            if (_recovered) {
                return;
            }
            _recovered = true;

            Logger.LogVerbose("Recover ref, messenger: {0}, {1}", gameObject.name, GetInstanceID());

            if (null != _assets) {
                foreach (var assetBase in _assets) {
                    Logger.LogVerbose("Recover ref, messenger: {0}, {1}, loader: {2}",
                        gameObject.name, GetInstanceID(), assetBase.LoaderPath);
                    assetBase.Retain();
                    Logger.LogVerbose("Recover ref, messenger: {0}, {1}, loader: {2}, finished!",
                        gameObject.name, GetInstanceID(), assetBase.LoaderPath);
                }
            }

            if (null != _typedAssets) {
                foreach (var kv in _typedAssets) {
                    Logger.LogVerbose("Recover ref, messenger: {0}, {1}, loader: {2}",
                        gameObject.name, GetInstanceID(), kv.Value.LoaderPath);
                    kv.Value.Retain();
                    Logger.LogVerbose("Recover ref, messenger: {0}, {1}, loader: {2}, finished",
                        gameObject.name, GetInstanceID(), kv.Value.LoaderPath);
                }
            }

            Logger.LogVerbose("Recover ref, messenger: {0}, {1}, finished!", gameObject.name, GetInstanceID());

            Messengers.Add(this);
        }

        private void OnDestroy()
        {
            ReleaseRef();
        }

        public void ReleaseRef()
        {
            if (null != _assets) {
                foreach (var asset in _assets) {
                    Logger.LogVerbose("Release ref from messenger: {0}, {1}, loader: {2}",
                        gameObject.name, GetInstanceID(), asset.LoaderPath);
                    asset.Release();
                    Logger.LogVerbose("Release ref from messenger: {0}, {1}, loader: {2}, finished!",
                        gameObject.name, GetInstanceID(), asset.LoaderPath);
                }
                HashSetPool<AssetBase>.Return(_assets);
            }

            if (null != _typedAssets) {
                foreach (var kv in _typedAssets) {
                    Logger.LogVerbose("Release ref from messenger: {0}, {1}, loader: {2}",
                        gameObject.name, GetInstanceID(), kv.Value.LoaderPath);
                    kv.Value.Release();
                    Logger.LogVerbose("Release ref from messenger: {0}, {1}, loader: {2}, finished!",
                        gameObject.name, GetInstanceID(), kv.Value.LoaderPath);
                }
                DictionaryPool<Type, AssetBase>.Return(_typedAssets);
            }

            Messengers.Remove(this);
        }

        public void RetainRef(AssetBase asset)
        {
            if (null == _assets)
                _assets = HashSetPool<AssetBase>.Get();

            if (_assets.Contains(asset))
                return;
            _assets.Add(asset);

            Logger.LogVerbose("Retain ref from messenger: {0}, {1}, loader: {2}",
                gameObject.name, GetInstanceID(), asset.LoaderPath);

            asset.Retain();

            Logger.LogVerbose("Retain ref from messenger: {0}, {1}, loader: {2}, finished!",
                gameObject.name, GetInstanceID(), asset.LoaderPath);

            Messengers.Add(this);
        }

        public void RetainRef<TSetter>(AssetBase asset) {
            if (null == _typedAssets)
                _typedAssets = DictionaryPool<Type, AssetBase>.Get();

            var type = typeof(TSetter);
            if (_typedAssets.ContainsKey(type)) {
                Logger.LogVerbose("Setter exist: {0}, release previous asset: {1}",
                    type.FullName, _typedAssets[type].AssetPath);

                _typedAssets[type].Release();

                Logger.LogVerbose("Setter exist: {0}, release previous asset: {1}, finished!",
                    type.FullName, _typedAssets[type].AssetPath);
            }
            _typedAssets[type] = asset;

            Logger.LogVerbose("Retain ref from messenger: {0}, {1}, loader: {2}",
                gameObject.name, GetInstanceID(), asset.LoaderPath);

            asset.Retain();

            Logger.LogVerbose("Retain ref from messenger: {0}, {1}, loader: {2}, finished!",
                gameObject.name, GetInstanceID(), asset.LoaderPath);

            Messengers.Add(this);
        }
    }
}