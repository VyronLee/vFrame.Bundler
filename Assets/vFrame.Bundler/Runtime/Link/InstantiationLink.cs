// ------------------------------------------------------------
//         File: InstantiationLink.cs
//        Brief: Non-exclusive asset link for instances created via Instantiate; many instances
//               may share one loader, which stays alive until all of them are released.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:04:43
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Non-exclusive link that tracks asset instances created via Instantiate; multiple links
    /// may reference the same loader concurrently.
    /// </summary>
    public class InstantiationLink : LinkBase
    {
        /// <summary>
        /// Always <c>false</c>: an instantiation link never exclusively owns its loader,
        /// so each created instance contributes an independent reference count.
        /// </summary>
        internal override bool Exclusive => false;
    }
}