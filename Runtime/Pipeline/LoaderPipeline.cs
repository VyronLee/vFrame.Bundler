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
using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    internal class LoaderPipeline
    {
        private readonly BundlerContexts _bundlerContexts;
        private readonly LoaderContexts _loaderContexts;
        private readonly List<Loader> _loaders;
        private int _processing;
        private bool _error;

        public LoaderPipeline(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) {
            _bundlerContexts = bundlerContexts;
            _loaderContexts = loaderContexts;
            _loaders = new List<Loader>();
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

        private Loader Last() {
            if (_loaders.Count <= 0) {
                return null;
            }
            return _loaders[_loaders.Count - 1];
        }

        private T Last<T>() where T: Loader {
            return Last() as T;
        }

        private Loader Processing() {
            if (_processing > 0 && _processing < _loaders.Count) {
                return _loaders[_processing];
            }
            return null;
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

        public bool IsDone => _processing >= _loaders.Count;
        public bool IsError => _error;
    }
}