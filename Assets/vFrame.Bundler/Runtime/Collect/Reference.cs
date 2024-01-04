// ------------------------------------------------------------
//         File: Reference.cs
//        Brief: Reference.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 13:1
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    public class Reference : IReference
    {
        private int _references;

        public virtual void Retain() {
            ++_references;
        }

        public virtual void Release() {
            --_references;
        }

        public virtual int GetReferences() {
            return _references;
        }
    }
}