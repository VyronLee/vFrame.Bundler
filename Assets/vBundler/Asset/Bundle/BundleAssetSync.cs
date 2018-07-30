using System;
using vBundler.Exception;
using vBundler.Loader;
using vBundler.Log;
using vBundler.Utils;

namespace vBundler.Asset.Bundle
{
    public class BundleAssetSync : AssetBase
    {
        public BundleAssetSync(string path, Type type, LoaderBase target) : base(path, type, target)
        {
        }

        protected override void LoadAssetInternal()
        {
            Logger.LogInfo("Start synchronously loading asset: " + _path);

            var name = PathUtility.GetAssetName(_path);
            _asset = _target.AssetBundle.LoadAsset(name, _type);
            if (!_asset)
                throw new BundleAssetLoadFailedException(
                    string.Format("Could not load asset {0} from assetbundle: ", name));
            IsDone = true;

            Logger.LogInfo("End synchronously loading asset: " + name);
        }
    }
}