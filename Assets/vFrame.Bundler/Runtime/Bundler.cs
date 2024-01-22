//------------------------------------------------------------
//        File:  Bundler.cs
//       Brief:  Bundler
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:18
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    public class Bundler : IBundler
    {
        private static readonly Type[] _embedSystems = {
            typeof(LogSystem),
            typeof(LoadSystem),
            typeof(LinkSystem),
            typeof(CollectSystem),
            typeof(ProfileSystem)
        };

        private readonly BundlerContexts _contexts;
        private readonly Dictionary<Type, BundlerSystem> _systems = new Dictionary<Type, BundlerSystem>();

        public Bundler(BundlerManifest manifest, BundlerOptions options = null) {
            options = options ?? new BundlerOptions();
            _contexts = new BundlerContexts {
                Options = options,
                Manifest = manifest,
                Bundler = this
            };
            InitializeSystems();
        }

        private void InitializeSystems() {
            var baseType = typeof(BundlerSystem);
            foreach (var type in _embedSystems) {
                if (!baseType.IsAssignableFrom(type)) {
                    throw new ArgumentException("Cannot create bundler system of type: " + type);
                }
                _systems[type] = Activator.CreateInstance(type, _contexts) as BundlerSystem;
            }

            if (null != _contexts.Options.LogHandler) {
                GetSystem<LogSystem>().SetLogHandler(_contexts.Options.LogHandler);
            }
        }

        public void Destroy() {
            foreach (var system in _systems.Select(kv => kv.Value)) {
                system.Destroy();
            }
            _systems.Clear();
        }

        internal T GetSystem<T>() where T: BundlerSystem {
            return _systems[typeof(T)] as T;
        }

        public Asset LoadAsset(string path, Type type) {
            return GetSystem<LoadSystem>().LoadAsset(path, type, AssetLoadType.LoadAsset);
        }

        public AssetAsync LoadAssetAsync(string path, Type type) {
            return GetSystem<LoadSystem>().LoadAssetAsync(path, type, AssetLoadType.LoadAsset);
        }

        public Asset LoadAssetWithSubAssets(string path, Type type) {
            return GetSystem<LoadSystem>().LoadAsset(path, type, AssetLoadType.LoadAssetWithSubAsset);
        }

        public AssetAsync LoadAssetWithSubAssetsAsync(string path, Type type) {
            return GetSystem<LoadSystem>().LoadAssetAsync(path, type, AssetLoadType.LoadAssetWithSubAsset);
        }

        public Asset<T> LoadAsset<T>(string path) where T : Object {
            return GetSystem<LoadSystem>().LoadAsset<T>(path, AssetLoadType.LoadAsset);
        }

        public AssetAsync<T> LoadAssetAsync<T>(string path) where T : Object {
            return GetSystem<LoadSystem>().LoadAssetAsync<T>(path, AssetLoadType.LoadAsset);
        }

        public Asset<T> LoadAssetWithSubAssets<T>(string path) where T : Object {
            return GetSystem<LoadSystem>().LoadAsset<T>(path, AssetLoadType.LoadAssetWithSubAsset);
        }

        public AssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object {
            return GetSystem<LoadSystem>().LoadAssetAsync<T>(path, AssetLoadType.LoadAssetWithSubAsset);
        }

        public Scene LoadScene(string path, LoadSceneMode mode) {
            return GetSystem<LoadSystem>().LoadScene(path, mode);
        }

        public SceneAsync LoadSceneAsync(string path, LoadSceneMode mode) {
            return GetSystem<LoadSystem>().LoadSceneAsync(path, mode);
        }

        public void Update() {
            foreach (var kv in _systems) {
                var system = kv.Value;
                system.Update();
            }
        }

        public void Collect() {
            //GetSystem<CollectSystem>().Collect();
        }

        public void SetLogLevel(int level) {
            GetSystem<LogSystem>().SetLogLevel(level);
        }
    }
}