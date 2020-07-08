//------------------------------------------------------------
//        File:  BundlerMessenger.cs
//       Brief:  BundlerMessenger
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 11:22
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using UnityEngine;
using vFrame.Bundler.Assets;
using vFrame.Bundler.Utils.Pools;

namespace vFrame.Bundler.Messengers
{
    public class BundlerMessenger : MonoBehaviour
    {
        public static readonly HashSet<BundlerMessenger> Messengers = new HashSet<BundlerMessenger>();

        private HashSet<AssetBase> _assets;

        private void Awake()
        {
            // Reference should be calculated when cloning from other instance
            RecoverRefs();

            hideFlags = HideFlags.HideAndDontSave;
        }

        private void RecoverRefs()
        {
            if (null == _assets)
                return;

            foreach (var assetBase in _assets)
                assetBase.Retain();

            Messengers.Add(this);
        }

        private void OnDestroy()
        {
            ReleaseRef();
        }

        public void ReleaseRef()
        {
            if (null == _assets)
                return;

            foreach (var assetBase in _assets)
                assetBase.Release();

            HashSetPool<AssetBase>.Return(_assets);

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
    }
}