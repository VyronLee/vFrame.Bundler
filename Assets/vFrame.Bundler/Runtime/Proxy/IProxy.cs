// ------------------------------------------------------------
//         File: IProxy.cs
//        Brief: IProxy.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 23:11
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal interface IProxy
    {
        Loader Loader { get; set; }
    }
}