using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vBundler.Asset.Bundle;
using vBundler.Asset.Resource;
using vBundler.Exception;
using vBundler.Interface;
using vBundler.Loader;
using vBundler.Loader.Factory;
using vBundler.Sequence;
using vBundler.Utils;
using Logger = vBundler.Log.Logger;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vBundler
{
    public class Bundler : IBundler
    {
        private static readonly Dictionary<string, LoaderBase> LoaderCache
            = new Dictionary<string, LoaderBase>();

        private readonly BundlerManifest _manifest;
        private readonly List<string> _searchPaths = new List<string>();

        private bool bundleMode = true;

        public Bundler(BundlerManifest manifest)
        {
            _manifest = manifest;

#if UNITY_EDITOR
            ReadEditorSetting();
#endif
        }

        public override void AddSearchPath(string path)
        {
            _searchPaths.Add(path);
        }

        public override void ClearSearchPaths()
        {
            _searchPaths.Clear();
        }

        public override IAsset Load<T>(string path)
        {
            if (bundleMode)
                return LoadFromBundle(path, typeof(T));
            return LoadFromResource(path, typeof(T));
        }

        public override IAsset Load(string path, Type type)
        {
            if (bundleMode)
                return LoadFromBundle(path, type);
            return LoadFromResource(path, type);
        }

        public override IEnumerator LoadAsync<T>(string path, AsyncLoadCallback callback)
        {
            if (bundleMode)
                yield return LoadFromBundleAsync(path, typeof(T), callback);
            else
                yield return LoadFromResourceAsync(path, typeof(T), callback);
        }

        public override IEnumerator LoadAsync(string path, Type type, AsyncLoadCallback callback)
        {
            if (bundleMode)
                yield return LoadFromBundleAsync(path, type, callback);
            else
                yield return LoadFromResourceAsync(path, type, callback);
        }

        private IAsset LoadFromBundle(string path, Type type)
        {
            path = PathUtility.NormalizePath(path);

            if (!_manifest.assets.ContainsKey(path))
                throw new BundleNoneConfigurationException("Asset path not specified: " + path);

            var assetData = _manifest.assets[path];

            var loader = CreateLoaderAndDependencies<LoaderSyncFactory>(assetData.bundle);

            var sequence = new SequenceSync(loader);
            if (!sequence.IsDone)
                throw new BundleLoadFailedException("Bundle load failed: " + assetData.bundle);

            var asset = new BundleAssetSync(path, type, loader);
            loader.Release();
            return asset;
        }

        private IEnumerator LoadFromBundleAsync(string path, Type type, AsyncLoadCallback callback)
        {
            path = PathUtility.NormalizePath(path);

            if (!_manifest.assets.ContainsKey(path))
                throw new BundleNoneConfigurationException("Asset path not specified: " + path);

            var assetData = _manifest.assets[path];

            var loader = CreateLoaderAndDependencies<LoaderAsyncFactory>(assetData.bundle);

            var sequence = new SequenceAsync(loader);
            yield return sequence;

            var asset = new BundleAssetAsync(path, type, loader);
            yield return asset;
            callback(asset);
            loader.Release();
        }

        private LoaderBase CreateLoaderAndDependencies<TFactory>(string path) where TFactory : LoaderFactory, new()
        {
            LoaderBase loader;
            if (LoaderCache.TryGetValue(path, out loader))
            {
                loader.Retain();
                return loader;
            }

            if (!_manifest.bundles.ContainsKey(path))
                throw new BundleNoneConfigurationException("Bundle path not specified: " + path);

            var bundleData = _manifest.bundles[path];
            var dependencies = new List<LoaderBase>();
            bundleData.dependencies.ForEach(v => dependencies.Add(CreateLoaderAndDependencies<TFactory>(v)));

            loader = new TFactory().CreateLoader(path, _searchPaths);
            loader.Dependencies = dependencies;

            LoaderCache.Add(path, loader);

            loader.Retain();
            return loader;
        }

        private IAsset LoadFromResource(string path, Type type)
        {
            return new ResourceAssetSync(path, type);
        }

        private IEnumerator LoadFromResourceAsync(string path, Type type, AsyncLoadCallback callback)
        {
            var asset = new ResourceAssetAsync(path, type);
            yield return asset;
            callback(asset);
        }

        public override void GarbageCollect()
        {
            var unused = new List<string>();
            foreach (var kv in LoaderCache)
            {
                var loader = kv.Value;
                if (loader.GetReferences() <= 0) unused.Add(kv.Key);
            }

            foreach (var name in unused)
            {
                var loader = LoaderCache[name];
                if (loader.IsLoading)
                    continue;

                Logger.LogVerbose(string.Format("Garbage collecting[{0}] - unloading {1}", Time.frameCount, name));
                loader.Unload();
                LoaderCache.Remove(name);
            }
        }

        public override void SetLogLevel(int level)
        {
            Logger.SetLogLevel(level);
        }

#if UNITY_EDITOR
        private void ReadEditorSetting()
        {
            bundleMode = EditorPrefs.GetBool(BundlerSetting.kBundlerModePreferenceKey, false);

            var logLevel = EditorPrefs.GetInt(BundlerSetting.kBundlerLogLevelPreferenceKey, Logger.LogLevel.ERROR - 1);
            SetLogLevel(logLevel + 1);
        }
#endif
    }
}