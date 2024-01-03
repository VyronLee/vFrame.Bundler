// ------------------------------------------------------------
//         File: BundlerRefObject.cs
//        Brief: BundlerRefObject.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 17:48
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal abstract class BundlerReferenceObject : BundlerObject, IReference
    {
        private int _references;

        protected BundlerReferenceObject(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _references = 0;
        }

        public override void Destroy() {
            _references = 0;
            base.Destroy();
        }

        public virtual void Retain() {
            ++_references;
        }

        public virtual void Release() {
            --_references;
        }

        public virtual int GetReferences() {
            return _references;
        }
    }
}