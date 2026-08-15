// ------------------------------------------------------------
//         File: ResourcesSceneLoaderAsync.cs
//        Brief: Empty subclass of RuntimeSceneLoaderAsync marking the Resources-mode async scene loader.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 22:21
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    internal class ResourcesSceneLoaderAsync : RuntimeSceneLoaderAsync
    {
        protected ResourcesSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}