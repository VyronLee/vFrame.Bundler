// ------------------------------------------------------------
//         File: AssetBundleSceneLoaderSync.cs
//        Brief: Sync scene loader for AssetBundle mode; inherits synchronous
//               LoadScene behavior from RuntimeSceneLoaderSync.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 23:43
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    internal class AssetBundleSceneLoaderSync : RuntimeSceneLoaderSync
    {
        public AssetBundleSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}