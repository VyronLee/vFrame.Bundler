// ------------------------------------------------------------
//         File: PropertyLink.cs
//        Brief: PropertyLink.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-4 21:21
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public abstract class PropertyLink<T1, T2> : LinkBase where T1 : Component where T2 : Object
    {
        public abstract void Set(T1 target, T2 asset);

        internal override bool Exclusive => true;
    }
}