// ------------------------------------------------------------
//         File: Reference.cs
//        Brief: Standalone reference counter implementing IReference: Retain/Release with throw on underflow.
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

        public virtual void Retain()
        {
            ++_references;
        }

        public virtual void Release()
        {
            if (_references <= 0) {
                throw new System.InvalidOperationException(
                    "Release() called more times than Retain(); reference count is already at zero.");
            }
            --_references;
        }

        public virtual int References => _references;
    }
}