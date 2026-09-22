// ------------------------------------------------------------
//         File: LoaderPipeline.cs
//        Brief: Drives a sequential chain of Loaders; tracks progress, error state, and JSON-serializable diagnostics.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:01:35
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Generic;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Sequential chain of loaders that are started and updated one after another until all finish,
    /// exposing progress and error state as JSON-serializable diagnostics.
    /// </summary>
    internal class LoaderPipeline : IJsonSerializable
    {
        /// <summary>Contexts of the bundler that owns this pipeline.</summary>
        private readonly BundlerContexts _bundlerContexts;
        /// <summary>Contexts inherited by each loader appended to this pipeline.</summary>
        private readonly LoaderContexts _loaderContexts;
        private readonly List<Loader> _loaders;
        /// <summary>Identifier for correlating this pipeline in diagnostics output.</summary>
        private readonly string _guid;
        /// <summary>Editor frame count at which this pipeline was created.</summary>
        private readonly int _createFrame;
        /// <summary>Index of the loader currently being processed; -1 before startup.</summary>
        private int _processing;
        private bool _error;

        /// <summary>
        /// Create an empty pipeline bound to the given bundler and loader contexts.
        /// </summary>
        /// <param name="bundlerContexts">Contexts of the owning bundler.</param>
        /// <param name="loaderContexts">Contexts inherited by loaders appended later.</param>
        public LoaderPipeline(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
        {
            _bundlerContexts = bundlerContexts;
            _loaderContexts = loaderContexts;
            _loaders = new List<Loader>();
            _guid = System.Guid.NewGuid().ToString();
            _createFrame = Time.frameCount;
            _processing = -1;
            _error = false;
        }

        /// <summary>
        /// Creates a loader of type <typeparamref name="T"/>, parenting it to the current last loader,
        /// and appends it. Sets the error flag instead of throwing when creation fails.
        /// </summary>
        /// <typeparam name="T">Type of loader to instantiate.</typeparam>
        public void Add<T>() where T : Loader
        {
            var loaderContexts = _loaderContexts;
            loaderContexts.ParentLoader = Last();
            var loader = Activator.CreateInstance(typeof(T), _bundlerContexts, loaderContexts) as T;
            if (null == loader || loader.IsError) {
                _error = true;
                return;
            }
            _loaders.Add(loader);
        }

        /// <summary>
        /// Appends an existing loader to the chain.
        /// Sets the error flag when the loader is null or has already failed.
        /// </summary>
        /// <param name="loader">Loader to append.</param>
        public void Add(Loader loader)
        {
            if (null == loader || loader.IsError) {
                _error = true;
                return;
            }
            _loaders.Add(loader);
        }

        /// <summary>
        /// Registers all queued loaders with the bundler and drives the chain until every loader
        /// has finished or an error occurs.
        /// </summary>
        /// <typeparam name="T">Expected type of the final loader.</typeparam>
        /// <param name="result">The last loader cast to <typeparamref name="T"/>, or default.</param>
        /// <returns>True when the chain started without errors; otherwise false.</returns>
        /// <exception cref="BundleException">No loaders have been added to the pipeline.</exception>
        public bool Startup<T>(out T result) where T : Loader
        {
            if (_loaders.Count <= 0) {
                throw new BundleException("No loaders in pipeline, please add some loaders first.");
            }

            if (IsError) {
                GetLogSystem().LogWarning("Pipeline will not startup due to some errors in loaders.");
                result = default;
                return false;
            }

            if (StartupLoaderQueue()) {
                result = Last<T>();
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Registers every queued loader with the bundler contexts and begins processing from the first one.
        /// </summary>
        /// <returns>True when the whole chain completed without error.</returns>
        private bool StartupLoaderQueue()
        {
            foreach (var loader in _loaders) {
                _bundlerContexts.AddLoader(loader);
            }
            _processing = 0;

            Update();

            return !IsError;
        }

        /// <summary>Returns the log system of the owning bundler.</summary>
        private LogSystem GetLogSystem()
        {
            return _bundlerContexts.Bundler.GetSystem<LogSystem>();
        }

        /// <summary>Returns the most recently added loader, or null when the pipeline is empty.</summary>
        public Loader Last()
        {
            if (_loaders.Count <= 0) {
                return null;
            }
            return _loaders[_loaders.Count - 1];
        }

        /// <summary>
        /// Returns the most recently added loader cast to <typeparamref name="T"/>,
        /// or null when the pipeline is empty or the loader has another type.
        /// </summary>
        /// <typeparam name="T">Expected type of the last loader.</typeparam>
        public T Last<T>() where T : Loader
        {
            return Last() as T;
        }

        /// <summary>
        /// Starts the next idle loader and advances through the chain until a loader is still
        /// processing or reports an error. Call once per frame after Startup until <see cref="IsDone"/>.
        /// </summary>
        public void Update()
        {
            while (_processing < _loaders.Count) {
                var loader = _loaders[_processing];
                switch (loader.TaskState) {
                    case TaskState.NotStarted:
                        loader.Start();
                        break;
                    case TaskState.Processing:
                        return;
                    case TaskState.Finished:
                        ++_processing;
                        break;
                    case TaskState.Error:
                        GetLogSystem().LogWarning("Error occurred while processing loader: {0}", loader);
                        _error = true;
                        return;
                }
            }
        }

        /// <summary>Identifier for correlating this pipeline in diagnostics output.</summary>
        [JsonSerializableProperty]
        public string Guid => _guid;

        /// <summary>Frame count at which this pipeline was created (editor diagnostics only).</summary>
        [JsonSerializableProperty]
        public int CreateFrame => _createFrame;

        [JsonSerializableProperty]
        public bool IsDone => _processing >= _loaders.Count;

        [JsonSerializableProperty]
        public bool IsError => _error;

        /// <summary>Index of the loader currently being processed; -1 before startup, <see cref="LoaderCount"/> when done.</summary>
        [JsonSerializableProperty]
        public int Processing => _processing;

        [JsonSerializableProperty]
        public int LoaderCount => _loaders?.Count ?? 0;

        /// <summary>Asset path reported by the final asset or scene loader, or null when unavailable.</summary>
        [JsonSerializableProperty]
        public string AssetPath => Last<AssetLoader>()?.AssetPath ?? Last<SceneLoader>()?.AssetPath;

        [JsonSerializableProperty]
        public List<Loader> Loaders => _loaders;

        /// <summary>Returns a compact diagnostic summary of the pipeline state.</summary>
        public override string ToString()
        {
            return $"[Guid: {Guid}, AssetPath: {AssetPath}, IsDone: {IsDone}, IsError: {IsError}, Processing: {Processing}, LoaderCount: {LoaderCount}]";
        }
    }
}