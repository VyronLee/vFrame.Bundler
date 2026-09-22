// ------------------------------------------------------------
//         File: TabbedPanel.cs
//        Brief: BindableElement tab page container with a UxmlFactory for direct placement in UXML layouts.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:25:29
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Minimal bindable element that serves as a tab page container; instances are managed by
    ///     <see cref="TabbedPanelGroup" /> and placed directly in UXML layouts.
    /// </summary>
    internal class TabbedPanel : BindableElement
    {
        /// <summary>UXML factory enabling <see cref="TabbedPanel" /> instantiation from UXML documents.</summary>
        public new class UxmlFactory : UxmlFactory<TabbedPanel, UxmlTraits> { }
    }
}

#endif