// ------------------------------------------------------------
//         File: ViewElementAttribute.cs
//        Brief: ViewElementAttribute.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-1 17:24
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using System;

namespace vFrame.Bundler
{
    internal class ViewElementAttribute : Attribute
    {
        public string Path { get; }

        public ViewElementAttribute(string path = null) {
            Path = path;
        }
    }
}

#endif