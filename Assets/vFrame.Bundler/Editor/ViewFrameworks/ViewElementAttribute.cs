// ------------------------------------------------------------
//         File: ViewElementAttribute.cs
//        Brief: Attribute marking a field or property for auto-binding to a UI Toolkit element queried by name/path.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:25:46
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using System;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Marks a field or property for auto-binding to a UI Toolkit element queried from the view root by
    ///     <see cref="Path" /> during <c>ViewBase</c> element binding.
    /// </summary>
    internal class ViewElementAttribute : Attribute
    {
        /// <summary>UI Toolkit element name or hierarchy path of the element to bind the member to.</summary>
        public string Path { get; }

        /// <summary>
        ///     Create the attribute with the element path to bind.
        /// </summary>
        /// <param name="path">UI Toolkit element name or hierarchy path; null when not specified.</param>
        public ViewElementAttribute(string path = null)
        {
            Path = path;
        }
    }
}

#endif