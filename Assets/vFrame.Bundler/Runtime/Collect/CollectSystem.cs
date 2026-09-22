// ------------------------------------------------------------
//         File: CollectSystem.cs
//        Brief: Garbage collection pass: destroys zero-reference loaders, unloaded handlers, finished
//               pipelines, and links whose target Object died; tears down everything on destroy.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:59:53
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Runs the bundler garbage-collection pass: destroys finished zero-reference loaders,
    ///     unloaded handlers, finished pipelines, and links whose target <see cref="Object"/> has
    ///     died, and tears down all tracked objects on destroy.
    /// </summary>
    internal class CollectSystem : BundlerSystem
    {
        /// <summary>Scratch list reused per pass; holds loaders collected for destruction.</summary>
        private readonly List<Loader> _nonReferenceLoaders;

        /// <summary>Scratch list reused per pass; holds handlers collected for removal.</summary>
        private readonly List<ILoaderHandler> _unloadedHandlers;

        /// <summary>Scratch list reused per pass; holds pipelines collected for removal.</summary>
        private readonly List<LoaderPipeline> _finishedPipelines;

        /// <summary>Scratch list reused per pass; holds links collected for removal.</summary>
        private readonly List<LinkBase> _destroyedLinks;

        /// <summary>
        ///     Creates the collection system.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler contexts this system operates on.</param>
        public CollectSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {
            _nonReferenceLoaders = new List<Loader>();
            _unloadedHandlers = new List<ILoaderHandler>();
            _finishedPipelines = new List<LoaderPipeline>();
            _destroyedLinks = new List<LinkBase>();
        }

        /// <summary>
        ///     Destroys every tracked loader, handler, pipeline, and link during bundler teardown.
        /// </summary>
        protected override void OnDestroy()
        {
            DestroyAllLoaders();
            DestroyAllHandlers();
            DestroyAllPipelines();
            DestroyAllLinks();
        }

        /// <summary>
        ///     Removes and destroys every loader registered in the bundler contexts.
        /// </summary>
        private void DestroyAllLoaders()
        {
            using (new ClearAtExist(_nonReferenceLoaders)) {
                BundlerContexts.ForEachLoader(v => _nonReferenceLoaders.Add(v));
                foreach (var loader in _nonReferenceLoaders) {
                    BundlerContexts.RemoveLoader(loader);
                    loader.Destroy();
                }
            }
        }

        /// <summary>
        ///     Removes every loader handler registered in the bundler contexts.
        /// </summary>
        private void DestroyAllHandlers()
        {
            using (new ClearAtExist(_unloadedHandlers)) {
                BundlerContexts.ForEachHandler(v => _unloadedHandlers.Add(v));
                foreach (var handler in _unloadedHandlers) {
                    BundlerContexts.RemoveHandler(handler);
                }
            }
        }

        /// <summary>
        ///     Removes every loader pipeline registered in the bundler contexts.
        /// </summary>
        private void DestroyAllPipelines()
        {
            using (new ClearAtExist(_finishedPipelines)) {
                BundlerContexts.ForEachPipeline(v => _finishedPipelines.Add(v));
                foreach (var pipeline in _finishedPipelines) {
                    BundlerContexts.RemovePipeline(pipeline);
                }
            }
        }

        /// <summary>
        ///     Removes every link (and its per-target link group) registered in the bundler contexts.
        /// </summary>
        private void DestroyAllLinks()
        {
            using (new ClearAtExist(_destroyedLinks)) {
                BundlerContexts.ForEachLinks((v, link) => _destroyedLinks.Add(link));
                foreach (var link in _destroyedLinks) {
                    var target = ((ILink)link).Target;
                    BundlerContexts.RemoveLinks(target);
                }
            }
        }

        /// <summary>
        ///     Intentionally empty; collection only runs when <see cref="Collect"/> is invoked.
        /// </summary>
        protected override void OnUpdate()
        {

        }

        /// <summary>
        ///     Runs one garbage-collection pass: removes loaders that are done with zero references,
        ///     handlers that finished unloading, pipelines that are done, and links whose target
        ///     <see cref="Object"/> has been destroyed.
        /// </summary>
        public void Collect()
        {
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

        /// <summary>
        ///     Queues the loader for destruction when it is done and holds no references.
        /// </summary>
        /// <param name="loader">The loader to inspect.</param>
        private void FilterNonReferenceLoader(Loader loader)
        {
            if (!loader.IsDone) {
                return;
            }
            if (loader.References > 0) {
                return;
            }
            _nonReferenceLoaders.Add(loader);
        }

        /// <summary>
        ///     Queues the handler for removal when it has finished unloading.
        /// </summary>
        /// <param name="handler">The handler to inspect.</param>
        private void FilterUnloadedHandler(ILoaderHandler handler)
        {
            if (handler.IsUnloaded) {
                _unloadedHandlers.Add(handler);
            }
        }

        /// <summary>
        ///     Queues the pipeline for removal when it has finished executing.
        /// </summary>
        /// <param name="pipeline">The pipeline to inspect.</param>
        private void FilterFinishedPipeline(LoaderPipeline pipeline)
        {
            if (pipeline.IsDone) {
                _finishedPipelines.Add(pipeline);
            }
        }

        /// <summary>
        ///     Queues the link for removal when its target <see cref="Object"/> has been destroyed.
        /// </summary>
        /// <param name="target">The link target to inspect.</param>
        /// <param name="link">The link to inspect.</param>
        private void FilterDestroyedLinks(Object target, LinkBase link)
        {
            if (!target) {
                _destroyedLinks.Add(link);
            }
        }

        /// <summary>
        ///     Removes and destroys the loaders queued by <see cref="FilterNonReferenceLoader"/>.
        /// </summary>
        private void CollectNonReferenceLoaders()
        {
            foreach (var loader in _nonReferenceLoaders) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing non-referenced loader: {0}", loader);
                BundlerContexts.RemoveLoader(loader);
                loader.Destroy();
            }
        }

        /// <summary>
        ///     Removes the handlers queued by <see cref="FilterUnloadedHandler"/>.
        /// </summary>
        private void CollectUnloadedHandlers()
        {
            foreach (var handler in _unloadedHandlers) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing unloaded handler: {0}", handler);
                BundlerContexts.RemoveHandler(handler);
            }
        }

        /// <summary>
        ///     Removes the pipelines queued by <see cref="FilterFinishedPipeline"/>.
        /// </summary>
        private void CollectFinishedPipelines()
        {
            foreach (var pipeline in _finishedPipelines) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing finished pipeline: {0}", pipeline);
                BundlerContexts.RemovePipeline(pipeline);
            }
        }

        /// <summary>
        ///     Removes the links queued by <see cref="FilterDestroyedLinks"/> and releases them.
        /// </summary>
        private void CollectDestroyedLinks()
        {
            foreach (var link in _destroyedLinks) {
                Facade.GetSystem<LogSystem>().LogInfo("Removing destroyed link: {0}", link);
                var target = ((ILink)link).Target;
                BundlerContexts.RemoveLinks(target);
                link.Release();
            }
        }
    }
}