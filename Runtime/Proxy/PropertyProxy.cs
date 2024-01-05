// ------------------------------------------------------------
//         File: PropertyProxy.cs
//        Brief: PropertyProxy.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-5 12:1
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    public class PropertyProxy : IProxy
    {
        Loader IProxy.Loader { get; set; }

        internal void Retain() {
            ((IProxy)this).Loader?.Retain();
        }

        internal void Release() {
            ((IProxy)this).Loader?.Release();
        }
    }
}