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

        public CollectSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _nonReferenceLoaders = new List<Loader>();
        }

        protected override void OnDestroy() {

        }

        protected override void OnUpdate() {

        }

        public void Collect() {
            using (new ClearAtExist(_nonReferenceLoaders)) {
                BundlerContexts.ForEachLoader(FilterNonReferenceLoader);
                CollectNonReferenceLoaders();
            }
        }

        private void FilterNonReferenceLoader(Loader loader) {
            if (!loader.IsDone) {
                return;
            }
            if (loader.GetReferences() > 0) {
                return;
            }
            _nonReferenceLoaders.Add(loader);
        }

        private void CollectNonReferenceLoaders() {
            foreach (var loader in _nonReferenceLoaders) {
                BundlerContexts.RemoveLoader(loader);
                loader.Destroy();
            }
        }
    }
}