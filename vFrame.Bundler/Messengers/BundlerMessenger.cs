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

            Logger.LogVerbose("Recover ref, messenger instance: {0}", GetInstanceID());

            if (null != _assets) {
                foreach (var assetBase in _assets)
                    assetBase.Retain();
            }

            if (null != _typedAssets) {
                foreach (var kv in _typedAssets)
                    kv.Value.Retain();
            }

            Messengers.Add(this);
        }

        private void OnDestroy()
        {
            ReleaseRef();
        }

        public void ReleaseRef()
        {
            if (null != _assets) {
                foreach (var assetBase in _assets)
                    assetBase.Release();
                HashSetPool<AssetBase>.Return(_assets);
            }

            if (null != _typedAssets) {
                foreach (var kv in _typedAssets)
                    kv.Value.Release();
                DictionaryPool<Type, AssetBase>.Return(_typedAssets);
            }

            Messengers.Remove(this);
        }

        public void RetainRef(AssetBase assetBase)
        {
            if (null == _assets)
                _assets = HashSetPool<AssetBase>.Get();

            if (_assets.Contains(assetBase))
                return;
            _assets.Add(assetBase);

            assetBase.Retain();

            Messengers.Add(this);
        }

        public void RetainRef<TSetter>(AssetBase assetBase) {
            if (null == _typedAssets)
                _typedAssets = DictionaryPool<Type, AssetBase>.Get();

            var type = typeof(TSetter);
            if (_typedAssets.ContainsKey(type)) {
                Logger.LogVerbose("Setter exist: {0}, release previous asset: {1}",
                    type.FullName, _typedAssets[type].AssetPath);

                _typedAssets[type].Release();
            }
            _typedAssets[type] = assetBase;

            assetBase.Retain();

            Messengers.Add(this);
        }
    }
}