//------------------------------------------------------------
//        File:  DestroyedMessenger.cs
//       Brief:  DestroyedMessenger
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 11:22
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using UnityEngine;
using vBundler.Asset;

namespace vBundler.Messenger
{
    public class DestroyedMessenger : MonoBehaviour
    {
        public static readonly LinkedList<DestroyedMessenger> Messengers = new LinkedList<DestroyedMessenger>();

        [SerializeField]
        private readonly List<AssetBase> _assets = new List<AssetBase>();

        private void Awake()
        {
            // Reference should be calculated when cloning from other instance
            RecoverRefs();
        }

        private void RecoverRefs()
        {
            foreach (var assetBase in _assets)
                assetBase.Retain();

            if (!Messengers.Contains(this))
                Messengers.AddLast(this);
        }

        private void OnDestroy()
        {
            ReleaseRef();
        }

        public void ReleaseRef()
        {
            foreach (var assetBase in _assets)
                assetBase.Release();
            _assets.Clear();

            Messengers.Remove(this);
        }

        public void RetainRef(AssetBase assetBase)
        {
            if (_assets.Contains(assetBase))
                return;
            _assets.Add(assetBase);

            assetBase.Retain();

            if (!Messengers.Contains(this))
                Messengers.AddLast(this);
        }
    }
}