using System;
using System.IO;
using UnityEngine;
using vBundler.Exception;
using vBundler.Loader;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Asset.Resource
{
    public class ResourceAssetSync : AssetBase
    {
        public ResourceAssetSync(string assetName, Type type, LoaderBase target = null)
            : base(assetName, type, target)
        {
        }

        protected override void LoadAssetInternal()
        {
            Logger.LogInfo("Start synchronously loading asset: " + _path);

            var resPath = PathUtility.RelativeProjectPathToRelativeResourcesPath(_path);
            var name = string.Format("{0}/{1}", Path.GetDirectoryName(resPath),
                Path.GetFileNameWithoutExtension(resPath));

            _asset = Resources.Load(name, _type);
            if (!_asset)
                throw new BundleAssetLoadFailedException("Could not load asset from resources: " + name);
            IsDone = true;

            Logger.LogInfo("End synchronously loading asset: " + _path);
        }
    }
}