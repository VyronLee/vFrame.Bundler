// ------------------------------------------------------------
//         File: CompletedLoader.cs
//        Brief: CompletedLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 17:50
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal class CompletedLoader : Loader
    {
        public CompletedLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {
        }

        protected override void OnDestroy() {

        }

        public override float Progress => 1f;

        protected override void OnStart() {

        }

        protected override void OnStop() {

        }

        protected override void OnUpdate() {
            Finish();
        }

        protected override void OnForceComplete() {
            Finish();
        }
    }
}