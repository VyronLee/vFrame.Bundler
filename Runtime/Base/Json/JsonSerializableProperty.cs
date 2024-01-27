// ------------------------------------------------------------
//         File: JsonSerializableProperty.cs
//        Brief: JsonSerializableProperty.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 20:4
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;

namespace vFrame.Bundler
{
    internal class JsonSerializableProperty : Attribute
    {
        private readonly string _format;

        public JsonSerializableProperty(string format = null) {
            _format = format;
        }

        public string Format => _format;
    }
}