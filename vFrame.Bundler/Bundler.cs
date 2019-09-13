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
using vFrame.Bundler.Interface;
using vFrame.Bundler.Logs;
using vFrame.Bundler.Modes;

namespace vFrame.Bundler
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

            Logger.LogInfo("Bundler manifest not provided, bundle mode will disabled.");

            if (manifest != null)
            {
                SetMode(BundleModeType.Bundle);
                return;
            }

            SetMode(BundleModeType.Resource);
        }

        private ModeBase CurrentMode
        {
            get { return _modes[_modeType]; }
        }

        public ILoadRequest Load(string path)
        {
            return CurrentMode.Load(path);
        }

        public ILoadRequestAsync LoadAsync(string path)
        {
            return CurrentMode.LoadAsync(path);
        }

        public void AddSearchPath(string path)
        {
            _searchPaths.Add(path);
        }

        public void ClearSearchPaths()
        {
            _searchPaths.Clear();
        }

        public void Collect()
        {
            CurrentMode.Collect();
        }

        public void DeepCollect()
        {
            CurrentMode.DeepCollect();
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