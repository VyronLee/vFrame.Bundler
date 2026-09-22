// ------------------------------------------------------------
//         File: BundlerSystem.cs
//        Brief: Base class for bundler subsystems; adds a per-frame Update()/OnUpdate() tick on top of BundlerObject.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:59:49
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Abstract base for bundler subsystems. Adds a per-frame <see cref="Update"/> tick on top of
    /// the <see cref="BundlerObject"/> lifecycle; derived classes implement <see cref="OnUpdate"/>.
    /// </summary>
    internal abstract class BundlerSystem : BundlerObject
    {
        /// <summary>
        /// Initializes the subsystem with the bundler-wide contexts.
        /// </summary>
        /// <param name="bundlerContexts">Contexts shared across the owning <see cref="Bundler"/>.</param>
        protected BundlerSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {

        }

        /// <summary>
        /// Advances the subsystem by one frame by invoking the <see cref="OnUpdate"/> hook.
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Performs the subsystem's per-frame work. Called once per frame from <see cref="Update"/>.
        /// </summary>
        protected abstract void OnUpdate();
    }
}