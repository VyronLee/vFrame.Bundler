// ------------------------------------------------------------
//         File: LinkSystem.cs
//        Brief: Manages asset links: instantiates assets with automatic InstantiationLink
//               registration, and releases/recreates exclusive PropertyLinks on components.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:04:51
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    /// Manages asset links: instantiating assets with automatic <see cref="InstantiationLink"/> registration,
    /// and releasing/recreating exclusive <see cref="PropertyLink{T1,T2}"/>s on component targets.
    /// </summary>
    internal class LinkSystem : BundlerSystem
    {
        /// <summary>
        /// Create the link system bound to the shared bundler contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared context collection used to store and query registered links.</param>
        public LinkSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {

        }

        /// <summary>
        /// Called when the system is destroyed. Performs no extra cleanup; links are released by their owners.
        /// </summary>
        protected override void OnDestroy()
        {

        }

        /// <summary>
        /// Called once per frame. Links are managed on demand, so no per-frame work is performed.
        /// </summary>
        protected override void OnUpdate()
        {

        }

        /// <summary>
        /// Instantiates the asset object held by the handler's loader and registers an
        /// <see cref="InstantiationLink"/> that keeps the loader retained until the link is released.
        /// </summary>
        /// <param name="handler">Loader handler whose loader must be an <see cref="AssetLoader"/>.</param>
        /// <param name="parent">Parent transform assigned to the new instance, or null for no parent.</param>
        /// <param name="stayWorldPosition">Whether the instance keeps its world position when parented.</param>
        /// <returns>The instantiated object, or null if the handler's loader is not an <see cref="AssetLoader"/>.</returns>
        public Object InstantiateAndLink(ILoaderHandler handler, Transform parent, bool stayWorldPosition)
        {
            var assetLoader = handler.Loader as AssetLoader;
            if (null == assetLoader) {
                Facade.GetSystem<LogSystem>().LogError("AssetLoader required, got: {0}",
                    handler.Loader?.GetType().Name ?? "null");
                return null;
            }

            var obj = Object.Instantiate(assetLoader.AssetObject, parent, stayWorldPosition);
            var instantiation = ObjectPool<InstantiationLink>.Get();

            var link = (ILink)instantiation;
            link.Loader = handler.Loader;
            link.Target = obj;

            instantiation.Retain();

            BundlerContexts.AddLink(instantiation);
            return obj;
        }

        /// <summary>
        /// Releases all existing <typeparamref name="TLink"/> links on the target component, then
        /// recreates a single link from the asset object held by the handler's loader.
        /// </summary>
        /// <param name="handler">Loader handler supplying the new asset; its loader must be an <see cref="AssetLoader"/>.</param>
        /// <param name="target">Component that receives the property link.</param>
        public void RelinkProperty<TComponent, TObject, TLink>(ILoaderHandler handler, TComponent target)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new()
        {

            ReleaseLinkedProperty<TComponent, TObject, TLink>(target);
            RecreateLink<TComponent, TObject, TLink>(handler, target);
        }

        /// <summary>
        /// Releases every <typeparamref name="TLink"/> registered on the component and removes them from the contexts.
        /// Does nothing when no matching links exist.
        /// </summary>
        /// <param name="component">Component whose links of the given type are released.</param>
        private void ReleaseLinkedProperty<TComponent, TObject, TLink>(TComponent component)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new()
        {

            if (!BundlerContexts.TryGetLinks<TLink>(component, out var links)) {
                return;
            }
            foreach (var link in links) {
                Facade.GetSystem<LogSystem>().LogInfo("Release linked property: {0}", link);
                link.Release();
            }
            BundlerContexts.RemoveLinksOfType<TLink>(component);
        }

        /// <summary>
        /// Creates a new <typeparamref name="TLink"/> from the handler's asset object and registers it on the
        /// component. Logs an error and does nothing if the loader is not an <see cref="AssetLoader"/>
        /// or the asset object does not match <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="handler">Loader handler supplying the asset to link.</param>
        /// <param name="component">Component that receives the property link.</param>
        private void RecreateLink<TComponent, TObject, TLink>(ILoaderHandler handler, TComponent component)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new()
        {

            var assetLoader = handler.Loader as AssetLoader;
            if (null == assetLoader) {
                Facade.GetSystem<LogSystem>().LogError("AssetLoader required, got: {0}",
                    handler.Loader?.GetType().Name ?? "null");
                return;
            }

            var asset = assetLoader.AssetObject as TObject;
            if (!asset) {
                Facade.GetSystem<LogSystem>().LogError("{0} required, got: {1}",
                    typeof(TObject).Name, assetLoader.AssetObject.GetType().Name);
                return;
            }

            var setter = ObjectPool<TLink>.Get();
            var proxy = (ILink)setter;
            proxy.Loader = handler.Loader;
            proxy.Target = component;

            setter.Set(component, asset);
            setter.Retain();

            BundlerContexts.AddLink(setter);
        }
    }
}