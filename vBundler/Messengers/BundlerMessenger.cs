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
using vBundler.Assets;
using vBundler.Utils.Pools;

namespace vBundler.Messengers
{
    public class BundlerMessenger : MonoBehaviour
    {
        public static readonly List<BundlerMessenger> Messengers = new List<BundlerMessenger>(2048);

        [SerializeField]
        private List<AssetBase> _assets;

        private void Awake()
        {
            // Reference should be calculated when cloning from other instance
            RecoverRefs();
        }

        private void RecoverRefs()
        {
            if (null == _assets)
                return;

            foreach (var assetBase in _assets)
                assetBase.Retain();

            if (!Messengers.Contains(this))
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

            ListPool<AssetBase>.Return(_assets);

            Messengers.Remove(this);
        }

        public void RetainRef(AssetBase assetBase)
        {
            if (null == _assets)
                _assets = ListPool<AssetBase>.Get();

            if (_assets.Contains(assetBase))
                return;
            _assets.Add(assetBase);

            assetBase.Retain();

            if (!Messengers.Contains(this))
                Messengers.Add(this);
        }
    }
}