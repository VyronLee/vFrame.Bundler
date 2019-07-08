//------------------------------------------------------------
//        File:  Bundler.cs
//       Brief:  Bundler
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:18
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using vBundler.Interface;
using vBundler.Log;
using vBundler.Mode;

namespace vBundler
{
    public class Bundler : IBundler
    {
        private readonly Dictionary<BundleModeType, ModeBase> _modes;
        private readonly List<string> _searchPaths = new List<string>();
        private BundleModeType _modeType;

        public Bundler(BundlerManifest manifest = null)
        {
            _modes = new Dictionary<BundleModeType, ModeBase>(2);
            _modes[BundleModeType.Bundle] = new BundleMode(manifest, _searchPaths);
            _modes[BundleModeType.Resource] = new ResourceMode(manifest, _searchPaths);

            if (manifest != null)
            {
                SetMode(BundleModeType.Bundle);
            }
            else
            {
                Logger.LogInfo("Bundler manifest not provided, bundle mode will disabled.");
                SetMode(BundleModeType.Resource);
            }
        }

        private ModeBase CurrentMode
        {
            get { return _modes[_modeType]; }
        }

        public ILoadRequest LoadAsset(string path)
        {
            return CurrentMode.LoadAsset(path);
        }

        public ILoadRequestAsync LoadAssetAsync(string path)
        {
            return CurrentMode.LoadAssetAsync(path);
        }

        public void AddSearchPath(string path)
        {
            _searchPaths.Add(path);
        }

        public void ClearSearchPaths()
        {
            _searchPaths.Clear();
        }

        public void GarbageCollect()
        {
            CurrentMode.GarbageCollect();
        }

        public void SetMode(BundleModeType type)
        {
            _modeType = type;
        }

        public void SetLogLevel(int level)
        {
            Logger.SetLogLevel(level);
        }
    }
}