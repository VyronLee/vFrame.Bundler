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
    public abstract class LoaderHandler : IReference
    {
        internal abstract Loader Loader { set; }

        public void Retain() {
            throw new System.NotImplementedException();
        }

        public void Release() {
            throw new System.NotImplementedException();
        }

        public int GetReferences() {
            throw new System.NotImplementedException();
        }
    }
}