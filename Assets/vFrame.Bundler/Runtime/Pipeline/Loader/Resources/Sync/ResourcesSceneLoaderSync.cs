// ------------------------------------------------------------
//         File: ResourcesSceneLoaderSync.cs
//        Brief: Empty subclass of RuntimeSceneLoaderSync marking the Resources-mode synchronous scene loader.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 20:3
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    internal class ResourcesSceneLoaderSync : RuntimeSceneLoaderSync
    {
        protected ResourcesSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}