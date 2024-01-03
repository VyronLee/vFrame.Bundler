// ------------------------------------------------------------
//         File: BundlerObject.cs
//        Brief: BundlerObject.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 15:38
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal abstract class BundlerObject
    {
        private readonly BundlerContexts _bundlerContexts;

        protected BundlerObject(BundlerContexts bundlerContexts) {
            _bundlerContexts = bundlerContexts;
        }

        public virtual void Destroy() {
            OnDestroy();
        }

        protected abstract void OnDestroy();

        protected BundlerContexts BundlerContexts => _bundlerContexts;
        protected Bundler Facade => _bundlerContexts.Bundler;
    }
}