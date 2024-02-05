// ------------------------------------------------------------
//         File: LinkSystem.cs
//        Brief: LinkSystem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 22:36
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class LinkSystem : BundlerSystem
    {
        public LinkSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {

        }

        protected override void OnDestroy() {

        }

        protected override void OnUpdate() {

        }

        public Object InstantiateAndLink(ILoaderHandler handler, Transform parent, bool stayWorldPosition) {
            var assetLoader = handler.Loader as AssetLoader;
            if (null == assetLoader) {
                Facade.GetSystem<LogSystem>().LogError("AssetLoader required, got: {0}",
                    handler.Loader?.GetType().Name ?? "null");
                return null;
            }

            var obj = Object.Instantiate(assetLoader.AssetObject, parent, stayWorldPosition);
            var instantiation = ObjectPool<InstantiationLink>.Get();

            var link = (ILink) instantiation;
            link.Loader = handler.Loader;
            link.Target = obj;

            instantiation.Retain();

            BundlerContexts.AddLink(instantiation);
            return obj;
        }

        public void RelinkProperty<TComponent, TObject, TLink>(ILoaderHandler handler, TComponent target)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new() {

            ReleaseLinkedProperty<TComponent, TObject, TLink>(target);
            RecreateLink<TComponent, TObject, TLink>(handler, target);
        }

        private void ReleaseLinkedProperty<TComponent, TObject, TLink>(TComponent component)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new() {

            if (!BundlerContexts.TryGetLinks<TLink>(component, out var links)) {
                return;
            }
            foreach (var link in links) {
                Facade.GetSystem<LogSystem>().LogInfo("Release linked property: {0}", link);
                link.Release();
            }
            BundlerContexts.RemoveLinksOfType<TLink>(component);
        }

        private void RecreateLink<TComponent, TObject, TLink>(ILoaderHandler handler, TComponent component)
            where TComponent : Component
            where TObject : Object
            where TLink : PropertyLink<TComponent, TObject>, new() {

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
            var proxy = (ILink) setter;
            proxy.Loader = handler.Loader;
            proxy.Target = component;

            setter.Set(component, asset);
            setter.Retain();

            BundlerContexts.AddLink(setter);
        }
    }
}