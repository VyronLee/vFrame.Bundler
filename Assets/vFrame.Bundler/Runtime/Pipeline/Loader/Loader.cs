// ------------------------------------------------------------
//         File: Loader.cs
//        Brief: Abstract reference-counted loader task with a Start/Stop/Update/ForceComplete state
//               machine, progress and elapsed-time reporting, and parent-loader retain/release.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:50:20
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Diagnostics;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Abstract reference-counted loader task driving an asset-load state machine.
    /// Subclasses implement <see cref="OnStart"/>, <see cref="OnStop"/>, <see cref="OnUpdate"/>
    /// and <see cref="OnForceComplete"/>; the base class tracks state, timing and the parent loader.
    /// </summary>
    internal abstract class Loader : BundlerReferenceObject, ITask, IJsonSerializable
    {
        private readonly LoaderContexts _loaderContexts;
        private readonly Stopwatch _stopwatch;
        private readonly int _createFrame;

        /// <summary>
        /// Initializes the loader, records the creation frame and retains the parent loader, if any.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts passed to the base object.</param>
        /// <param name="loaderContexts">Per-load contexts, including the optional parent loader.</param>
        protected Loader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) : base(bundlerContexts)
        {
            _loaderContexts = loaderContexts;
            _stopwatch = new Stopwatch();
            _createFrame = Time.frameCount;
            RetainParent();
        }

        /// <summary>Per-load contexts, including the optional parent loader.</summary>
        protected LoaderContexts LoaderContexts => _loaderContexts;

        /// <summary>Current state-machine state of this loader task.</summary>
        [JsonSerializableProperty(true)]
        public TaskState TaskState { get; private set; }

        /// <summary>Frame count (<see cref="Time.frameCount"/>) at which this loader was created.</summary>
        [JsonSerializableProperty]
        public int CreateFrame => _createFrame;

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            Facade.GetSystem<LogSystem>().LogInfo("Loader destroyed: {0}", this);
            ReleaseParent();
            Stop();
        }

        /// <summary>
        /// Transitions from <see cref="TaskState.NotStarted"/> to <see cref="TaskState.Processing"/>
        /// and starts the elapsed-time stopwatch; no-op in any other state.
        /// </summary>
        public void Start()
        {
            if (TaskState != TaskState.NotStarted) {
                return;
            }
            TaskState = TaskState.Processing;
            _stopwatch.Start();
            OnStart();
        }

        /// <summary>
        /// Transitions from <see cref="TaskState.Processing"/> or <see cref="TaskState.Finished"/>
        /// back to <see cref="TaskState.NotStarted"/>, stops the stopwatch and invokes
        /// <see cref="OnStop"/>; no-op in any other state.
        /// </summary>
        public void Stop()
        {
            if (TaskState != TaskState.Processing && TaskState != TaskState.Finished) {
                return;
            }
            TaskState = TaskState.NotStarted;
            _stopwatch.Stop();
            OnStop();
        }

        /// <summary>
        /// Invokes <see cref="OnUpdate"/> once, only while the loader is in
        /// <see cref="TaskState.Processing"/>; no-op in any other state.
        /// </summary>
        public void Update()
        {
            if (TaskState != TaskState.Processing) {
                return;
            }
            OnUpdate();
        }

        /// <summary>Whether the loader has reached <see cref="TaskState.Finished"/>.</summary>
        public bool IsDone => TaskState == TaskState.Finished;

        /// <summary>Whether the loader has reached <see cref="TaskState.Error"/>.</summary>
        public bool IsError => TaskState == TaskState.Error;

        /// <summary>Load progress in the range [0, 1]; semantics defined by subclasses.</summary>
        public abstract float Progress { get; }

        /// <summary>Elapsed wall-clock milliseconds since <see cref="Start"/> was called.</summary>
        [JsonSerializableProperty]
        public double Elapsed => _stopwatch.Elapsed.TotalMilliseconds;

        /// <summary>
        /// Transitions the loader into <see cref="TaskState.Error"/> and stops the stopwatch.
        /// </summary>
        protected void Abort()
        {
            TaskState = TaskState.Error;
            _stopwatch.Stop();
            Facade.GetSystem<LogSystem>().LogError("Loader abort: {0}", this);
        }

        /// <summary>
        /// Transitions the loader into <see cref="TaskState.Finished"/> and stops the stopwatch.
        /// </summary>
        protected void Finish()
        {
            TaskState = TaskState.Finished;
            _stopwatch.Stop();
            Facade.GetSystem<LogSystem>().LogInfo("Loader finished: {0}", this);
        }

        /// <summary>
        /// Throws if the loader has not reached <see cref="TaskState.Finished"/>.
        /// </summary>
        /// <exception cref="BundleAssetNotReadyException">The loader is not finished.</exception>
        protected void ThrowIfNotFinished()
        {
            if (TaskState != TaskState.Finished) {
                throw new BundleAssetNotReadyException($"Loader has not finished: {this}");
            }
        }

        /// <summary>
        /// Synchronously drives the loader to <see cref="TaskState.Finished"/> by repeatedly
        /// starting and force-completing it; logs a warning and returns if the state is not
        /// startable or processing.
        /// </summary>
        public void ForceComplete()
        {
            while (true) {
                switch (TaskState) {
                    case TaskState.NotStarted:
                        Start();
                        break;
                    case TaskState.Processing:
                        Facade.GetSystem<LogSystem>().LogInfo("Force loader complete: {0}", this);
                        OnForceComplete();
                        break;
                    case TaskState.Finished:
                        return;
                    default:
                        Facade.GetSystem<LogSystem>().LogWarning(
                            "Cannot force loading because task is not processing: {0}", this);
                        return;
                }
            }
        }

        /// <summary>Called once when the loader starts; subclass initialization hook.</summary>
        protected abstract void OnStart();

        /// <summary>Called once when the loader stops; subclass teardown hook.</summary>
        protected abstract void OnStop();

        /// <summary>Called per <see cref="Update"/> while processing; drives the actual load.</summary>
        protected abstract void OnUpdate();

        /// <summary>Called by <see cref="ForceComplete"/> to finish the load synchronously.</summary>
        protected abstract void OnForceComplete();

        /// <summary>Retains the parent loader so it outlives this loader.</summary>
        private void RetainParent()
        {
            LoaderContexts.ParentLoader?.Retain();
        }

        /// <summary>Releases the reference on the parent loader taken by the constructor.</summary>
        private void ReleaseParent()
        {
            LoaderContexts.ParentLoader?.Release();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, TaskState: {TaskState}]";
        }
    }
}