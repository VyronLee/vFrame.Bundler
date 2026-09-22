// ------------------------------------------------------------
//         File: BundlerReferenceObject.cs
//        Brief: Retain/Release reference counting; underflow throws, no-op after Destroy keeps cascades order-safe.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:55:24
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Abstract bundler object with reference counting. <see cref="Retain"/>/<see cref="Release"/> must be
    /// paired; releasing a live (non-destroyed) object at zero count throws. After <see cref="Destroy"/>,
    /// both operations become no-ops so destroy cascades stay order-safe.
    /// </summary>
    internal abstract class BundlerReferenceObject : BundlerObject, IReference
    {
        /// <summary>
        /// Current reference count; zeroed when destroyed.
        /// </summary>
        private int _references;

        /// <summary>
        /// Convergence flag: true once destroyed, turning Retain/Release into graceful no-ops.
        /// </summary>
        private bool _destroyed;

        /// <summary>
        /// Initializes the object with a zero reference count.
        /// </summary>
        /// <param name="bundlerContexts">Contexts shared across the owning <see cref="Bundler"/>.</param>
        protected BundlerReferenceObject(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {
            _references = 0;
        }

        /// <summary>
        /// Tears down the object and zeroes the reference count. Subsequent <see cref="Retain"/> and
        /// <see cref="Release"/> calls become no-ops, keeping the destroy cascade order-safe.
        /// </summary>
        public override void Destroy()
        {
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

        /// <summary>
        /// Increments the reference count. No-op if the object has been destroyed.
        /// </summary>
        public virtual void Retain()
        {
            if (_destroyed) {
                return;
            }
            ++_references;
        }

        /// <summary>
        /// Decrements the reference count. No-op if the object has been destroyed.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The reference count is already at zero.</exception>
        public virtual void Release()
        {
            if (_destroyed) {
                return;
            }
            if (_references <= 0) {
                throw new System.InvalidOperationException(
                    "Release() called more times than Retain(); reference count is already at zero.");
            }
            --_references;
        }

        /// <summary>
        /// Current reference count; always zero after destruction. Serialized for the profiler.
        /// </summary>
        [JsonSerializableProperty]
        public virtual int References => _references;
    }
}