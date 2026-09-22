// ------------------------------------------------------------
//         File: IReference.cs
//        Brief: Reference-counting contract: paired Retain/Release calls with a live reference count.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:59:58
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Reference-counting contract for shared resources. Owners pair <see cref="Retain"/> and
    /// <see cref="Release"/> calls around every outstanding use of the object.
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// Increments the reference count, claiming one outstanding use of the object.
        /// </summary>
        void Retain();

        /// <summary>
        /// Decrements the reference count, surrendering one outstanding use. Must be paired
        /// with a prior <see cref="Retain"/> call.
        /// </summary>
        void Release();

        /// <summary>
        /// Current number of outstanding <see cref="Retain"/> claims; zero means unreferenced.
        /// </summary>
        int References { get; }
    }
}