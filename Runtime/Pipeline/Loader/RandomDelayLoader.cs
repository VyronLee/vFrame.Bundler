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

namespace vFrame.Bundler
{
    internal class RandomDelayLoader : Loader
    {
        private readonly int _startFrame;
        private readonly int _frameLength;

        public RandomDelayLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts) : base(bundlerContexts, loaderContexts) {
            _startFrame = Time.frameCount;
            _frameLength = Random.Range(
                bundlerContexts.Options.MinAsyncFrameCountOnSimulation,
                bundlerContexts.Options.MaxAsyncFrameCountOnSimulation);
        }

        public override float Progress => Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f);

        protected override void OnStart() {

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
    }
}