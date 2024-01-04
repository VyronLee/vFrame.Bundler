// ------------------------------------------------------------
//         File: LoaderHandler.cs
//        Brief: LoaderHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 21:7
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal interface ILoaderHandler
    {
        internal Loader Loader { set; get; }
        internal void Update();
    }
}