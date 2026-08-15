// ------------------------------------------------------------
//         File: IReference.cs
//        Brief: Reference-counting contract with Retain/Release and a References counter.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:06:13
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    public interface IReference
    {
        void Retain();

        void Release();

        int References { get; }
    }
}