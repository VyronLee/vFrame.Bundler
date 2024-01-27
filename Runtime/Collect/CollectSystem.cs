// ------------------------------------------------------------
//         File: CollectSystem.cs
//        Brief: CollectSystem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 21:18
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class CollectSystem : BundlerSystem
    {
        private readonly List<Loader> _nonReferenceLoaders;
        private readonly List<ILoaderHandler> _unloadedHandlers;

        public CollectSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _nonReferenceLoaders = new List<Loader>();
            _unloadedHandlers = new List<ILoaderHandler>();
        }

        protected override void OnDestroy() {
            _nonReferenceLoaders.Clear();
            _unloadedHandlers.Clear();
        }

        protected override void OnUpdate() {

        }

        public void Collect() {
            using (new ClearAtExist(_nonReferenceLoaders)) {
                BundlerContexts.ForEachLoader(FilterNonReferenceLoader);
                CollectNonReferenceLoaders();
            }
            using (new ClearAtExist(_unloadedHandlers)) {
                BundlerContexts.ForEachHandler(FilterUnloadedHandler);
                CollectUnloadedHandlers();
            }
        }

        private void FilterNonReferenceLoader(Loader loader) {
            if (!loader.IsDone) {
                return;
            }
            if (loader.References > 0) {
                return;
            }
            _nonReferenceLoaders.Add(loader);
        }

        private void FilterUnloadedHandler(ILoaderHandler handler) {
            if (handler.IsUnloaded) {
                _unloadedHandlers.Add(handler);
            }
        }

        private void CollectNonReferenceLoaders() {
            foreach (var loader in _nonReferenceLoaders) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing non-referenced loader: {0}", loader);
                BundlerContexts.RemoveLoader(loader);
                loader.Destroy();
            }
        }

        private void CollectUnloadedHandlers() {
            foreach (var handler in _unloadedHandlers) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing unloaded handler: {0}", handler);
                BundlerContexts.RemoveHandler(handler);
            }
        }
    }
}