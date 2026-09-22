// ------------------------------------------------------------
//         File: BundlerObject.cs
//        Brief: Base class for bundler internals. Provides access to the shared BundlerContexts and the
//               Bundler facade; Destroy() triggers the OnDestroy() teardown hook.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:55:15
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Base class for bundler internal objects. Holds the shared <see cref="BundlerContexts"/>
    /// and exposes the <see cref="Bundler"/> facade, with a Destroy/OnDestroy teardown lifecycle.
    /// </summary>
    internal abstract class BundlerObject
    {
        /// <summary>
        /// Shared contexts (facade, providers, etc.) owned by the <see cref="Bundler"/> instance.
        /// </summary>
        private readonly BundlerContexts _bundlerContexts;

        /// <summary>
        /// Initializes the object with the bundler-wide contexts.
        /// </summary>
        /// <param name="bundlerContexts">Contexts shared across the owning <see cref="Bundler"/>.</param>
        protected BundlerObject(BundlerContexts bundlerContexts)
        {
            _bundlerContexts = bundlerContexts;
        }

        /// <summary>
        /// Tears down the object by invoking the <see cref="OnDestroy"/> hook.
        /// </summary>
        public virtual void Destroy()
        {
            OnDestroy();
        }

        /// <summary>
        /// Releases resources owned by the derived object. Called once from <see cref="Destroy"/>.
        /// </summary>
        protected abstract void OnDestroy();

        /// <summary>
        /// Contexts shared across the owning <see cref="Bundler"/> instance.
        /// </summary>
        protected BundlerContexts BundlerContexts => _bundlerContexts;

        /// <summary>
        /// The <see cref="Bundler"/> facade this object belongs to.
        /// </summary>
        protected Bundler Facade => _bundlerContexts.Bundler;
    }
}