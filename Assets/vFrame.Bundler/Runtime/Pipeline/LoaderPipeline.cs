// ------------------------------------------------------------
//         File: LoaderPipeline.cs
//        Brief: LoaderPipeline.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 20:43
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    internal class LoaderPipeline : IJsonSerializable
    {
        private readonly BundlerContexts _bundlerContexts;
        private readonly LoaderContexts _loaderContexts;
        private readonly List<Loader> _loaders;
        private readonly string _guid;
        private readonly int _createFrame;
        private int _processing;
        private bool _error;

        public LoaderPipeline(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) {
            _bundlerContexts = bundlerContexts;
            _loaderContexts = loaderContexts;
            _loaders = new List<Loader>();
            _guid = System.Guid.NewGuid().ToString();
            _createFrame = Time.frameCount;
            _processing = -1;
            _error = false;
        }

        public void Add<T>() where T : Loader {
            var loaderContexts = _loaderContexts;
            loaderContexts.ParentLoader = Last();
            var loader = Activator.CreateInstance(typeof(T), _bundlerContexts, loaderContexts) as T;
            if (null == loader || loader.IsError) {
                _error = true;
                return;
            }
            _loaders.Add(loader);
        }

        public bool Startup<T>(out T result) where T: Loader {
            if (_loaders.Count <= 0) {
                throw new BundleException("No loaders in pipeline, please add some loaders first.");
            }

            if (IsError) {
                GetLogSystem().LogWarning("Pipeline will not startup due to some errors in loaders.");
                result = default;
                return false;
            }

            if (StartupLoaderQueue()) {
                result = Last<T>();
                return true;
            }
            result = default;
            return false;
        }

        private bool StartupLoaderQueue() {
            foreach (var loader in _loaders) {
                _bundlerContexts.AddLoader(loader);
            }
            _processing = 0;

            Update();

            return !IsError;
        }

        private LogSystem GetLogSystem() {
            return _bundlerContexts.Bundler.GetSystem<LogSystem>();
        }

        public Loader Last() {
            if (_loaders.Count <= 0) {
                return null;
            }
            return _loaders[_loaders.Count - 1];
        }

        public T Last<T>() where T: Loader {
            return Last() as T;
        }

        public void Update() {
            while (_processing < _loaders.Count) {
                var loader = _loaders[_processing];
                switch (loader.TaskState) {
                    case TaskState.NotStarted:
                        loader.Start();
                        break;
                    case TaskState.Processing:
                        return;
                    case TaskState.Finished:
                        ++_processing;
                        break;
                    case TaskState.Error:
                        GetLogSystem().LogWarning("Error occurred while processing loader: {0}", loader);
                        _error = true;
                        return;
                }
            }
        }

        [JsonSerializableProperty]
        public string Guid => _guid;

        [JsonSerializableProperty]
        public int CreateFrame => _createFrame;

        [JsonSerializableProperty]
        public bool IsDone => _processing >= _loaders.Count;

        [JsonSerializableProperty]
        public bool IsError => _error;

        [JsonSerializableProperty]
        public int Processing => _processing;

        [JsonSerializableProperty]
        public int LoaderCount => _loaders?.Count ?? 0;

        [JsonSerializableProperty]
        public string AssetPath => Last<AssetLoader>()?.AssetPath ?? Last<SceneLoader>()?.AssetPath;

        [JsonSerializableProperty]
        public List<Loader> Loaders => _loaders;

        public override string ToString() {
            return $"[Guid: {Guid}, AssetPath: {AssetPath}, IsDone: {IsDone}, IsError: {IsError}, Processing: {Processing}, LoaderCount: {LoaderCount}]";
        }
    }
}