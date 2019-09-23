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
using UnityEngine;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Modes;
using Logger = vFrame.Bundler.Logs.Logger;

namespace vFrame.Bundler
{
    public class Bundler : IBundler
    {
        private Dictionary<BundleModeType, ModeBase> _modes;
        private readonly List<string> _searchPaths = new List<string>();
        private BundleModeType _modeType;

        public Bundler(string json)
        {
            BundlerManifest manifest = null;
            if (!string.IsNullOrEmpty(json))
                manifest = JsonUtility.FromJson<BundlerManifest>(json);
            Initialize(manifest);
        }

        public Bundler(BundlerManifest manifest = null)
        {
            Initialize(manifest);
        }

        private void Initialize(BundlerManifest manifest)
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