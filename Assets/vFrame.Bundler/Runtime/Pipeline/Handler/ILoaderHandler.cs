// ------------------------------------------------------------
//         File: ILoaderHandler.cs
//        Brief: Internal contract pairing a Loader with its owned BundlerContexts; supports per-frame
//               updates, unloading, and unloaded-state queries.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:20:28
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Internal handler contract binding a <see cref="Loader"/> to its <see cref="BundlerContexts"/>,
    /// driving per-frame updates and unload lifecycle.
    /// </summary>
    internal interface ILoaderHandler : IJsonSerializable
    {
        /// <summary>
        /// Gets or sets the loader instance this handler manages.
        /// </summary>
        Loader Loader { set; get; }

        /// <summary>
        /// Gets or sets the bundler contexts owned by this handler.
        /// </summary>
        BundlerContexts BundlerContexts { set; get; }

        /// <summary>
        /// Ticks the handler for the current frame.
        /// </summary>
        void Update();

        /// <summary>
        /// Begins unloading the handler's resources.
        /// </summary>
        /// <returns>The operation tracking unload progress.</returns>
        UnloadOperation Unload();

        /// <summary>
        /// Gets a value indicating whether the handler has fully unloaded.
        /// </summary>
        bool IsUnloaded { get; }
    }
}