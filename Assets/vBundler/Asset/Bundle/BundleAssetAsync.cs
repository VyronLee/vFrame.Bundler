using System;
using UnityEngine;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Loader;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Asset.Bundle
{
    public class BundleAssetAsync : AssetBase, IAssetAsync
    {
        private AssetBundleRequest _request;

        public BundleAssetAsync(string path, Type type, LoaderBase target) : base(path, type, target)
        {
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        public override bool IsDone
        {
            get
            {
                if (_asset)
                    return true;

                if (_request == null || !_request.isDone)
                    return false;

                _asset = _request.asset;

                Logger.LogInfo("End asynchronously loading asset: " + _path);

                return true;
            }
        }

        protected override void LoadAssetInternal()
        {
            Logger.LogInfo("Start asynchronously loading asset: " + _path);

            var name = PathUtility.GetAssetName(_path);
            _request = _target.AssetBundle.LoadAssetAsync(name, _type);
            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from assetbundle: ", name));
        }
    }
}