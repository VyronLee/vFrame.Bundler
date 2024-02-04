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
using UnityEngine;

namespace vFrame.Bundler
{
    internal class CollectSystem : BundlerSystem
    {
        private readonly List<Loader> _nonReferenceLoaders;
        private readonly List<ILoaderHandler> _unloadedHandlers;
        private readonly List<LoaderPipeline> _finishedPipelines;
        private readonly List<LinkBase> _destroyedLinks;

        public CollectSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _nonReferenceLoaders = new List<Loader>();
            _unloadedHandlers = new List<ILoaderHandler>();
            _finishedPipelines = new List<LoaderPipeline>();
            _destroyedLinks = new List<LinkBase>();
        }

        protected override void OnDestroy() {
            _nonReferenceLoaders.Clear();
            _unloadedHandlers.Clear();
            _finishedPipelines.Clear();
            _destroyedLinks.Clear();
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
            using (new ClearAtExist(_finishedPipelines)) {
                BundlerContexts.ForEachPipeline(FilterFinishedPipeline);
                CollectFinishedPipelines();
            }
            using (new ClearAtExist(_destroyedLinks)) {
                BundlerContexts.ForEachLinks(FilterDestroyedLinks);
                CollectDestroyedLinks();
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

        private void FilterFinishedPipeline(LoaderPipeline pipeline) {
            if (pipeline.IsDone) {
                _finishedPipelines.Add(pipeline);
            }
        }

        private void FilterDestroyedLinks(Object target, LinkBase link) {
            if (!target) {
                _destroyedLinks.Add(link);
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

        private void CollectFinishedPipelines() {
            foreach (var pipeline in _finishedPipelines) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing finished pipeline: {0}", pipeline);
                BundlerContexts.RemovePipeline(pipeline);
            }
        }

        private void CollectDestroyedLinks() {
            foreach (var link in _destroyedLinks) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing destroyed link: {0}", link);
                var target = ((ILink)link).Target;
                BundlerContexts.RemoveLinks(target);
                link.Release();
            }
        }
    }
}