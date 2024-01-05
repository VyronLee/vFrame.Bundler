// ------------------------------------------------------------
//         File: LinkSystem.cs
//        Brief: LinkSystem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 22:36
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal class LinkSystem : BundlerSystem
    {
        private readonly List<(Loader, Object)> _destroyedObjects;
        private readonly Action<Loader, HashSet<Object>> _searchDestroyedObjectAction;

        public LinkSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _destroyedObjects = new List<(Loader, Object)>();
            _searchDestroyedObjectAction = SearchDestroyedObject;
        }

        protected override void OnDestroy() {
            _destroyedObjects.Clear();
        }

        protected override void OnUpdate() {
            ValidateLinkedObjects();
        }

        public void LinkObject(Object target, ILoaderHandler handler) {
            handler.Loader.Retain();
            BundlerContexts.AddLinkedObject(handler.Loader, target);
        }

        private void ValidateLinkedObjects() {
            using (new ClearAtExist(_destroyedObjects)) {
                BundlerContexts.ForEachLinkedObject(_searchDestroyedObjectAction);
                RemoveDestroyedLinkedObjects();
            }
        }

        private void SearchDestroyedObject(Loader loader, HashSet<Object> objects) {
            foreach (var obj in objects) {
                if (!obj) {
                    _destroyedObjects.Add((loader, obj));
                }
            }
        }

        private void RemoveDestroyedLinkedObjects() {
            foreach (var tuple in _destroyedObjects) {
                var (loader, destroyedObject) = tuple;
                BundlerContexts.RemoveLinkedObject(loader, destroyedObject);
                loader.Release();
            }
        }

        public void RebindProxy<TComponent, TObject, TProxy>(ILoaderHandler handler, TComponent target)
            where TComponent : Component
            where TObject : Object
            where TProxy : PropertySetterProxy<TComponent, TObject>, new() {

            ReleaseCurrentProxy<TProxy>(target);
            CreateAndBindProxy<TComponent, TObject, TProxy>(handler, target);
        }

        private void ReleaseCurrentProxy<TProxy>(Component component) {
            if (!BundlerContexts.TryGetProxy(component, typeof(TProxy), out var current)) {
                return;
            }
            BundlerContexts.RemoveProxy(component, typeof(TProxy));
            current.Release();
        }

        private void CreateAndBindProxy<TComponent, TObject, TProxy>(ILoaderHandler handler, TComponent target)
            where TComponent : Component
            where TObject : Object
            where TProxy : PropertySetterProxy<TComponent, TObject>, new() {

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

            var setter = ObjectPool<TProxy>.Get();
            var proxy = (IProxy) setter;
            proxy.Loader = handler.Loader;
            setter.Set(target, asset);
            setter.Retain();

            BundlerContexts.AddProxy(target, setter);
        }
    }
}