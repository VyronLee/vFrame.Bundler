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

namespace vFrame.Bundler
{
    internal readonly struct LoaderPipeline
    {
        private readonly BundlerContexts _bundlerContexts;
        private readonly LoaderContexts _loaderContexts;
        private readonly List<Loader> _loaders;

        public LoaderPipeline(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) {
            _bundlerContexts = bundlerContexts;
            _loaderContexts = loaderContexts;
            _loaders = new List<Loader>();
        }

        public LoaderPipeline Add<T>() where T : Loader {
            var loader = Activator.CreateInstance(typeof(T), _bundlerContexts, _loaderContexts) as T;
            _bundlerContexts.Loaders.Add(loader);
            _loaders.Add(loader);
            return this;
        }

        public T Startup<T>() where T: LoaderHandler, new() {
            foreach (var loader in _loaders) {
                loader.Start();
            }
            var ret = new T { Loader = Last() };
            _bundlerContexts.LoaderHandlers.Add(ret);
            return ret;
        }

        private Loader Last() {
            if (_loaders.Count <= 0) {
                return null;
            }
            return _loaders[_loaders.Count - 1];
        }
    }
}