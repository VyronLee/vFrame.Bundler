// ------------------------------------------------------------
//         File: RandomDelayLoader.cs
//        Brief: Simulated loader that delays completion by a random frame count; used in editor simulation mode.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:50:28
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;
using Random = UnityEngine.Random;

namespace vFrame.Bundler
{
    /// <summary>
    /// Loader that simulates asynchronous loading by finishing after a random number of frames,
    /// faking async latency in editor simulation mode.
    /// </summary>
    internal class RandomDelayLoader : Loader
    {
        /// <summary>Frame count (<see cref="Time.frameCount"/>) at which the simulated load started.</summary>
        private int _startFrame;

        /// <summary>Simulated load duration in frames, randomized within the configured simulation range.</summary>
        private readonly int _frameLength;

        /// <summary>Unique identifier of this loader instance, used for logging and JSON output.</summary>
        private readonly string _guid;

        /// <summary>
        /// Initializes the loader and picks the random simulated frame duration.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts supplying the simulation frame range.</param>
        /// <param name="loaderContexts">Per-load contexts passed to the base loader.</param>
        public RandomDelayLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) : base(bundlerContexts, loaderContexts)
        {
            _frameLength = Random.Range(
                bundlerContexts.Options.MinAsyncFrameCountOnSimulation,
                bundlerContexts.Options.MaxAsyncFrameCountOnSimulation);
            _guid = System.Guid.NewGuid().ToString();
        }

        /// <summary>Frame-based progress in the range [0, 1], clamped once the simulated duration has elapsed.</summary>
        [JsonSerializableProperty]
        public override float Progress => Mathf.Min((float)(Time.frameCount - _startFrame) / _frameLength, 1f);

        /// <summary>Records the frame at which the simulated load starts.</summary>
        protected override void OnStart()
        {
            _startFrame = Time.frameCount;
        }

        /// <summary>No teardown is required; the simulated load holds no resources.</summary>
        protected override void OnStop()
        {

        }

        /// <summary>Finishes the loader once the simulated frame duration has elapsed.</summary>
        protected override void OnUpdate()
        {
            if (Time.frameCount - _startFrame < _frameLength) {
                return;
            }
            Finish();
        }

        /// <summary>Finishes the loader immediately, skipping the remaining simulated frames.</summary>
        protected override void OnForceComplete()
        {
            Finish();
        }

        /// <summary>Unique identifier of this loader instance, exposed for logging and JSON output.</summary>
        [JsonSerializableProperty]
        public string Guid => _guid;

        /// <summary>Returns a diagnostic string with guid, start frame, duration, state and progress.</summary>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, Guid: {Guid}, StartFrame: {_startFrame}, FrameLength: {_frameLength}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}