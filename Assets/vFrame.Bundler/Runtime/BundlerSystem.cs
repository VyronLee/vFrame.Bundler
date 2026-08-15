// ------------------------------------------------------------
//         File: BundlerSystem.cs
//        Brief: Base class for bundler subsystems: adds per-frame Update() over BundlerObject teardown.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 22:9
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    internal abstract class BundlerSystem : BundlerObject
    {
        protected BundlerSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {

        }

        public void Update()
        {
            OnUpdate();
        }

        protected abstract void OnUpdate();
    }
}