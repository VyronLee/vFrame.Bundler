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
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Modes;
using Logger = vFrame.Bundler.Logs.Logger;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vFrame.Bundler
{
    public class Bundler : IBundler
    {
        private Dictionary<BundleModeType, ModeBase> _modes;
        private readonly List<string> _searchPaths = new List<string>();
        private BundleModeType _modeType;
        private BundlerManifest _manifest;

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
            _manifest = manifest;

            _modes = new Dictionary<BundleModeType, ModeBase>(2);
            _modes[BundleModeType.Bundle] = new BundleMode(manifest, _searchPaths);
            _modes[BundleModeType.Resource] = new ResourceMode(manifest, _searchPaths);

            var bundleMode = false;
            var logLevel = Logger.LogLevel.ERROR;
#if UNITY_EDITOR
            bundleMode = EditorPrefs.GetBool("vFrameBundlerModePreferenceKey", false);
            logLevel = EditorPrefs.GetInt("vFrameBundlerLogLevelPreferenceKey", Logger.LogLevel.ERROR - 1) + 1;
#endif
            if (bundleMode)
            {
                if (manifest != null)
                {
                    SetMode(BundleModeType.Bundle);
                    return;
                }
                Logger.LogInfo("Bundle manifest does not provided, bundle mode will disable.");
            }
            SetMode(BundleModeType.Resource);
            SetLogLevel(logLevel);
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
            if (type == BundleModeType.Bundle && null == _manifest)
                throw new BundleException("Bundle manifest does not provided, bundle mode cannot enabled.");
            _modeType = type;
        }

        public void SetLogLevel(int level)
        {
            Logger.SetLogLevel(level);
        }
    }
}