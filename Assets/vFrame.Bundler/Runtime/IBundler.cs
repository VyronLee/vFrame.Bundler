// ------------------------------------------------------------
//         File: IBundler.cs
//        Brief: Public contract of the Bundler asset loading system: asset/scene loading, per-frame
//               update/collect, log level.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:04:34
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Public contract of the bundler: reference-counted asset and scene loading, per-frame driving, and log level.
    /// </summary>
    public interface IBundler
    {
        /// <summary>
        ///     Destroys all systems. The bundler must not be used afterwards.
        /// </summary>
        void Destroy();

        /// <summary>
        ///     Loads a single main asset at the given path.
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded asset alive.</returns>
        Asset LoadAsset(string path, Type type);

        /// <summary>
        ///     Loads a single main asset at the given path asynchronously.
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the asset to load.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        AssetAsync LoadAssetAsync(string path, Type type);

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets (e.g. sprites of a sheet, meshes of an FBX).
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the main asset to load.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded assets alive.</returns>
        Asset LoadAssetWithSubAssets(string path, Type type);

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets asynchronously.
        /// </summary>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <param name="type">Type of the main asset to load.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        AssetAsync LoadAssetWithSubAssetsAsync(string path, Type type);

        /// <summary>
        ///     Loads a single main asset at the given path, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded asset alive.</returns>
        Asset<T> LoadAsset<T>(string path) where T : Object;

        /// <summary>
        ///     Loads a single main asset at the given path asynchronously, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        AssetAsync<T> LoadAssetAsync<T>(string path) where T : Object;

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the main asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper that keeps the loaded assets alive.</returns>
        Asset<T> LoadAssetWithSubAssets<T>(string path) where T : Object;

        /// <summary>
        ///     Loads a main asset together with all of its sub-assets asynchronously, typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the main asset to load.</typeparam>
        /// <param name="path">Asset path as listed in the manifest.</param>
        /// <returns>Reference-counted wrapper tracking the asynchronous load.</returns>
        AssetAsync<T> LoadAssetWithSubAssetsAsync<T>(string path) where T : Object;

        /// <summary>
        ///     Loads the scene at the given path.
        /// </summary>
        /// <param name="path">Scene path as listed in the manifest.</param>
        /// <param name="mode">Scene loading mode, single or additive.</param>
        /// <returns>Wrapper for the loaded scene.</returns>
        Scene LoadScene(string path, LoadSceneMode mode);

        /// <summary>
        ///     Loads the scene at the given path asynchronously.
        /// </summary>
        /// <param name="path">Scene path as listed in the manifest.</param>
        /// <param name="mode">Scene loading mode, single or additive.</param>
        /// <returns>Wrapper tracking the asynchronous scene load.</returns>
        SceneAsync LoadSceneAsync(string path, LoadSceneMode mode);

        /// <summary>
        ///     Updates all systems; must be called every frame before <see cref="Collect"/>.
        /// </summary>
        void Update();

        /// <summary>
        ///     Releases assets whose references dropped to zero; call every frame after <see cref="Update"/>.
        /// </summary>
        void Collect();

        /// <summary>
        ///     Sets the verbosity level of the bundler logger.
        /// </summary>
        /// <param name="level">Log level value interpreted by the log system.</param>
        void SetLogLevel(int level);
    }
}