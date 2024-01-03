// ------------------------------------------------------------
//         File: AssetLoader.cs
//        Brief: AssetLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 18:0
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using Object = UnityEngine.Object;

namespace vFrame.Bundler
{
    internal abstract class AssetLoader : Loader
    {
        protected AssetLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

            AssetLoadType = loaderContexts.AssetLoadType;
            AssetPath = loaderContexts.AssetPath;
            AssetType = loaderContexts.AssetType;
        }

        protected string AssetPath { get; }
        protected Type AssetType { get; }
        protected AssetLoadType AssetLoadType { get; }
        public abstract Object AssetObject { get; }
    }
}