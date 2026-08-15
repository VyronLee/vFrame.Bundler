// ------------------------------------------------------------
//         File: AssetBundleSceneLoaderAsync.cs
//        Brief: Async scene loader for AssetBundle mode; inherits LoadSceneAsync behavior
//               from RuntimeSceneLoaderAsync.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 22:19
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    internal class AssetBundleSceneLoaderAsync : RuntimeSceneLoaderAsync
    {
        public AssetBundleSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}