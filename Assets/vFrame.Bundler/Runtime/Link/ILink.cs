// ------------------------------------------------------------
//         File: ILink.cs
//        Brief: Binds a loaded asset target Object to the Loader that loaded it, so the collect system can release it.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:04:38
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Binds a loaded asset target Object to the Loader that loaded it, so the collect system can
    ///     track and release the target through its owning Loader.
    /// </summary>
    internal interface ILink
    {
        /// <summary>
        ///     The Loader that loaded <see cref="Target" />; retained/released by the link implementation.
        /// </summary>
        Loader Loader { get; set; }

        /// <summary>
        ///     The loaded asset Object this link tracks (a GameObject instantiation, component setter, etc.).
        /// </summary>
        Object Target { get; set; }
    }
}