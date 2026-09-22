// ------------------------------------------------------------
//         File: IAssetBundleCreateAdapter.cs
//        Brief: Abstraction for opening AssetBundles (sync and async), pluggable to support VFS or other file sources.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:29:48
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Opens AssetBundle files from a given path, synchronously or asynchronously.
    ///     Implemented by the built-in adapter and can be replaced (e.g. VFS adapter) to change the file source.
    /// </summary>
    public interface IAssetBundleCreateAdapter
    {
        /// <summary>
        ///     Synchronously loads and returns the AssetBundle at the given path.
        /// </summary>
        /// <param name="bundlePath">Path of the bundle file, resolved against the active file source.</param>
        /// <returns>The loaded <see cref="AssetBundle"/>, or null if it could not be loaded.</returns>
        AssetBundle CreateAssetBundle(string bundlePath);

        /// <summary>
        ///     Starts an asynchronous load of the AssetBundle at the given path.
        /// </summary>
        /// <param name="bundlePath">Path of the bundle file, resolved against the active file source.</param>
        /// <returns>The request object whose <c>asset</c> yields the loaded <see cref="AssetBundle"/> when done.</returns>
        AssetBundleCreateRequest CreateRequest(string bundlePath);
    }
}