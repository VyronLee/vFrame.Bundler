using System;
using System.IO;
using UnityEngine;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Loader;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Asset.Resource
{
    public class ResourceAssetAsync : AssetBase, IAssetAsync
    {
        private ResourceRequest _request;

        public ResourceAssetAsync(string assetName, Type type, LoaderBase target = null) : base(assetName, type, target)
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

            var resPath = PathUtility.RelativeProjectPathToRelativeResourcesPath(_path);
            var name = string.Format("{0}/{1}", Path.GetDirectoryName(resPath),
                Path.GetFileNameWithoutExtension(resPath));

            _request = Resources.LoadAsync(name, _type);
            if (_request == null)
                throw new BundleAssetLoadFailedException(
                    string.Format("Cannot load asset {0} from resources: ", name));
        }
    }
}