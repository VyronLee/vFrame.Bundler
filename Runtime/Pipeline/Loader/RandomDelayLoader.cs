// ------------------------------------------------------------
//         File: RandomDelayLoader.cs
//        Brief: RandomDelayLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 19:21
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;
using Random = UnityEngine.Random;

namespace vFrame.Bundler
{
    internal class RandomDelayLoader : Loader
    {
        private int _startFrame;
        private readonly int _frameLength;
        private readonly string _guid;

        public RandomDelayLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) : base(bundlerContexts, loaderContexts) {
            _frameLength = Random.Range(
                bundlerContexts.Options.MinAsyncFrameCountOnSimulation,
                bundlerContexts.Options.MaxAsyncFrameCountOnSimulation);
            _guid = System.Guid.NewGuid().ToString();
        }

        [JsonSerializableProperty("F3")]
        public override float Progress => Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f);

        protected override void OnStart() {
            _startFrame = Time.frameCount;
        }

        protected override void OnStop() {

        }

        protected override void OnUpdate() {
            if (Time.frameCount - _startFrame < _frameLength) {
                return;
            }
            Finish();
        }

        protected override void OnForceComplete() {
            Finish();
        }

        [JsonSerializableProperty]
        public string Guid => _guid;

        public override string ToString() {
            return $"[@TypeName: {GetType().Name}, Guid: {Guid}, StartFrame: {_startFrame}, FrameLength: {_frameLength}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}