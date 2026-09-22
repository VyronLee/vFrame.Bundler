// ------------------------------------------------------------
//         File: Scene.cs
//        Brief: Scene loader handler: unloads asynchronously (Edit-mode closes the scene), activates the loaded
//               scene, and releases the loader retain once unloading finishes.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:20:32
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    /// Loader handler for a scene asset: activates the loaded scene, unloads it asynchronously
    /// (or closes it directly in edit mode), and releases the loader retain once unload completes.
    /// </summary>
    public class Scene : ILoaderHandler
    {
        /// <summary>The cached async scene-unload request started by <see cref="Unload"/>.</summary>
        private AsyncOperation _request;

        /// <summary>The awaitable returned to callers; created lazily on first play-mode unload.</summary>
        private UnloadOperation _unloadOperation;

        /// <summary>True once the unload finished and the loader retain was released.</summary>
        private bool _unloaded;

        /// <summary>The loader backing this handler; retained when assigned.</summary>
        private Loader _loader;

        /// <summary>The editor frame count at which this handler was created (serialized for diagnostics).</summary>
        private readonly int _createFrame = Time.frameCount;

        /// <summary>
        /// Gets the typed scene loader bound to this handler.
        /// </summary>
        /// <exception cref="ArgumentException">The bound loader is missing or is not a <see cref="SceneLoader"/>.</exception>
        internal SceneLoader SceneLoader => ((ILoaderHandler)this).Loader as SceneLoader
                                            ?? throw new ArgumentException("SceneLoader expected, got: "
                                                                           + (((ILoaderHandler)this).Loader?.GetType().Name ?? "null"));

        /// <summary>
        /// Gets or sets the loader backing this handler; assigning it retains the loader.
        /// </summary>
        Loader ILoaderHandler.Loader {
            get => _loader;
            set {
                _loader = value;
                _loader.Retain();
            }
        }

        /// <summary>
        /// Gets or sets the bundler contexts owned by this handler.
        /// </summary>
        BundlerContexts ILoaderHandler.BundlerContexts { get; set; }

        /// <summary>
        /// Begins unloading the scene. In edit mode outside play mode the scene is closed
        /// immediately and a pre-completed operation is returned; otherwise an async unload
        /// is started and the returned operation completes once unloading finishes.
        /// </summary>
        /// <returns>The operation tracking unload completion.</returns>
        public UnloadOperation Unload()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneLoader.SceneObject, true);
                return UnloadOperation.Completed;
            }
#endif
            _request = SceneManager.UnloadSceneAsync(SceneLoader.AssetPath);
            return _unloadOperation ?? (_unloadOperation = new UnloadOperation());
        }

        /// <summary>
        /// Activates the loaded scene as the active scene.
        /// </summary>
        /// <exception cref="InvalidOperationException">The scene object is no longer valid.</exception>
        public void Activate()
        {
            if (!SceneLoader.SceneObject.IsValid()) {
                throw new InvalidOperationException("Scene invalid: " + SceneLoader.AssetPath);
            }
            SceneManager.SetActiveScene(SceneLoader.SceneObject);
        }

        /// <summary>
        /// Ticks unload-progress tracking for the current frame.
        /// </summary>
        void ILoaderHandler.Update()
        {
            UpdateUnloadProcess();
        }

        /// <summary>
        /// Releases the loader and marks the unload operation done once the async unload
        /// request completes; runs at most once.
        /// </summary>
        private void UpdateUnloadProcess()
        {
            if (_unloaded) {
                return;
            }
            if (null == _request || !_request.isDone) {
                return;
            }
            _loader.Release();
            _unloadOperation?.SetDone(true);
            _unloaded = true;
        }

        /// <summary>
        /// Retains the underlying loader reference.
        /// </summary>
        public void Retain()
        {
            _loader?.Retain();
        }

        /// <summary>
        /// Releases the underlying loader reference.
        /// </summary>
        public void Release()
        {
            _loader?.Release();
        }

        /// <summary>
        /// Gets a value indicating whether the scene has finished unloading.
        /// </summary>
        [JsonSerializableProperty]
        public bool IsUnloaded => _unloaded;

        /// <summary>
        /// Gets the frame count at which this handler was created.
        /// </summary>
        [JsonSerializableProperty]
        public int CreateFrame => _createFrame;

        /// <summary>
        /// Gets the asset path of the scene handled by this handler.
        /// </summary>
        [JsonSerializableProperty]
        public string AssetPath => SceneLoader?.AssetPath;

        /// <summary>
        /// Returns a debug string containing the handler type and asset path.
        /// </summary>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, AssetPath: {SceneLoader.AssetPath}]";
        }
    }
}