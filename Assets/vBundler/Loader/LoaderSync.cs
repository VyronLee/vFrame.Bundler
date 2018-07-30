using System.Collections.Generic;
using System.IO;
using UnityEngine;
using vBundler.Exception;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Loader
{
    public class LoaderSync : LoaderBase
    {
        public LoaderSync(string path, List<string> searchPaths) : base(path, searchPaths)
        {
        }

        protected override bool LoadProcess()
        {
            Logger.LogInfo("Start synchronously loading process: " + _path);

            foreach (var basePath in _searchPaths)
            {
                var path = Path.Combine(basePath, _path);
                path = PathUtility.NormalizePath(path);

                try
                {
                    _assetBundle = AssetBundle.LoadFromFile(path);
                    if (_assetBundle)
                    {
                        IsDone = true;
                        Logger.LogInfo("AssetBundle synchronously loading finished, path: " + _path);
                        break;
                    }

                    IsDone = false;
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
                catch (System.Exception)
                {
                    Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                }
            }

            if (!IsDone)
                throw new BundleNotFoundException("AssetBundle synchronously loading failed, path: " + _path);

            Logger.LogInfo("End synchronously loading process: " + _path);

            return IsDone;
        }
    }
}