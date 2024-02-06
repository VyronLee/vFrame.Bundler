// ------------------------------------------------------------
//         File: ILink.cs
//        Brief: ILink.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 23:11
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    internal interface ILink
    {
        Loader Loader { get; set; }
        Object Target { get; set; }
    }
}