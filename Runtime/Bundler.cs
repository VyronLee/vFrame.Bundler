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
        };

        private readonly BundlerContexts _contexts;
        private readonly Dictionary<Type, BundlerSystem> _systems = new Dictionary<Type, BundlerSystem>();

        public Bundler(BundlerManifest manifest = null, BundlerOptions options = null) {
            options = options ?? new BundlerOptions();
            _contexts = new BundlerContexts {
                Options = options,
                Manifest = manifest,
                Bundler = this
            };
            InitializeEmbedSystems();
        }

        private void InitializeEmbedSystems() {
            var baseSystemType = typeof(BundlerSystem);
            foreach (var type in _embedSystems) {
                if (!baseSystemType.IsAssignableFrom(type)) {
                    throw new ArgumentException("Cannot create bundler system of type: " + type);
                }
                _systems[type] = Activator.CreateInstance(type, _contexts) as BundlerSystem;
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

        public IAsset LoadAsset(string path, Type type) {
            throw new NotImplementedException();
        }

        public IAssetAsync LoadAssetAsync(string path, Type type) {
            throw new NotImplementedException();
        }

        public IAsset LoadAssetWithSubAssets(string path, Type type) {
            throw new NotImplementedException();
        }

        public IAssetAsync LoadAssetWithSubAssetsAsync(string path, Type type) {
            throw new NotImplementedException();
        }

        public IAsset<T> LoadAsset<T>(string path) where T : Object {
            throw new NotImplementedException();
        }

        public IAssetAsync<T> LoadAssetAsync<T>(string path) where T : Object {
            throw new NotImplementedException();
        }

        public IAsset<T> LoadAssetWithSubAssets<T>(string path) where T : Object {
            throw new NotImplementedException();
        }

        public IAssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object {
            throw new NotImplementedException();
        }

        public IScene LoadScene(string path, LoadSceneMode mode) {
            throw new NotImplementedException();
        }

        public ISceneAsync LoadSceneAsync(string path, LoadSceneMode mode) {
            throw new NotImplementedException();
        }

        public void Update() {
            throw new NotImplementedException();
        }

        public void Collect() {
        }

        public void SetLogLevel(int level) {
        }
    }
}