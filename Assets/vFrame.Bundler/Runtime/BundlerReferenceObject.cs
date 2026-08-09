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
        private bool _destroyed;

        protected BundlerReferenceObject(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _references = 0;
        }

        public override void Destroy() {
            // Convergence flag: once destroyed, the ref-count machinery becomes a
            // graceful no-op. This makes the Destroy() cascade order-safe — a child
            // loader whose OnDestroy() runs after its parent's Destroy() and calls
            // ReleaseParent() (parent.Release()) will not underflow the parent's
            // already-zeroed count. Legitimate underflow detection for live
            // (non-destroyed) objects is preserved below in Release().
            _destroyed = true;
            _references = 0;
            base.Destroy();
        }

        public virtual void Retain() {
            if (_destroyed) {
                return;
            }
            ++_references;
        }

        public virtual void Release() {
            if (_destroyed) {
                return;
            }
            if (_references <= 0) {
                throw new System.InvalidOperationException(
                    "Release() called more times than Retain(); reference count is already at zero.");
            }
            --_references;
        }

        [JsonSerializableProperty]
        public virtual int References => _references;
    }
}