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

        public LoaderPipeline(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) {
            _bundlerContexts = bundlerContexts;
            _loaderContexts = loaderContexts;
            _loaders = new List<Loader>();
            _processing = -1;
        }

        public LoaderPipeline Add<T>() where T : Loader {
            var loaderContexts = _loaderContexts;
            loaderContexts.ParentLoader = Last();
            var loader = Activator.CreateInstance(typeof(T), _bundlerContexts, loaderContexts) as T;
            _bundlerContexts.AddLoader(loader);
            _loaders.Add(loader);
            return this;
        }

        public T Startup<T>() where T: Loader {
            if (_loaders.Count <= 0) {
                throw new BundleException("No loaders in pipeline, please add some loaders first.");
            }
            _processing = 0;
            Update();
            return Last<T>();
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
                        goto End;
                    case TaskState.Finished:
                        ++_processing;
                        break;
                    case TaskState.Error:
                        goto End;
                }
            }
            End: ;
        }

        public bool IsDone => _processing >= _loaders.Count;
        public bool IsError => Processing()?.TaskState == TaskState.Error;
    }
}