using System.Collections.Generic;
using System.IO;
using UnityEngine;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;

namespace vBundler.Loader
{
    public class LoaderAsync : LoaderBase, IAsync
    {
        private AssetBundleCreateRequest _request;

        public LoaderAsync(string path, List<string> searchPaths) : base(path, searchPaths)
        {
            _request = null;
        }

        public override bool IsDone
        {
            get
            {
                if (_assetBundle)
                    return true;

                if (_request == null)
                {
                    foreach (var basePath in _searchPaths)
                    {
                        var path = Path.Combine(basePath, _path);
                        path = PathUtility.NormalizePath(path);

                        try
                        {
                            _request = AssetBundle.LoadFromFileAsync(path);
                            if (_request != null)
                                return false;

                            Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        }
                        catch (System.Exception)
                        {
                            Logger.LogInfo("AssetBundle cannot load at path: {0}, searching next ... ", path);
                        }
                    }

                    throw new BundleLoadFailedException("Cannot load assetbundle: " + _path);
                }

                if (!_request.isDone)
                    return false;

                _assetBundle = _request.assetBundle;

                Logger.LogInfo("Add assetbundle to cache: " + _path);

                AssetBundleCache.Add(_path, _assetBundle);

                Logger.LogInfo("AssetBundle asynchronously loading finished, path: " + _path);

                IsLoading = false;
                return true;
            }
        }

        protected override bool LoadProcess()
        {
            Logger.LogInfo("Start asynchronously loading process: " + _path);

            IsLoading = true;
            return false;
        }

        #region IEnumerator

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        #endregion
    }
}