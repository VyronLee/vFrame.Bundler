// ------------------------------------------------------------
//         File: TabbedPanel.cs
//        Brief: TabbedPanel.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 17:48
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class TabbedPanel : BindableElement
    {
        public new class UxmlFactory : UxmlFactory<TabbedPanel, UxmlTraits> {}
    }
}

#endif