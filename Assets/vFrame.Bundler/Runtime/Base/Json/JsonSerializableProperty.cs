// ------------------------------------------------------------
//         File: JsonSerializableProperty.cs
//        Brief: Attribute marking an instance property for JSON serialization, with an optional format string for
//               IFormattable values.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:33:57
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Marks an instance property to be exported by the JSON serialization extension methods.
    ///     Properties without this attribute are skipped during serialization.
    /// </summary>
    internal class JsonSerializableProperty : Attribute
    {
        private readonly string _format;
        private readonly bool _formatToString;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonSerializableProperty" /> attribute.
        /// </summary>
        /// <param name="formatToString">
        ///     When <c>true</c>, an <see cref="IFormattable" /> property value is serialized as the
        ///     string produced by <see cref="IFormattable.ToString(string, IFormatProvider)" />; otherwise the raw value is kept.
        /// </param>
        /// <param name="format">The format string passed to <see cref="IFormattable.ToString(string, IFormatProvider)" />; may be <c>null</c>.</param>
        public JsonSerializableProperty(bool formatToString = false, string format = null)
        {
            _formatToString = formatToString;
            _format = format;
        }

        /// <summary>
        ///     Gets a value indicating whether <see cref="IFormattable" /> property values are serialized using
        ///     <see cref="Format" /> instead of being written as raw values.
        /// </summary>
        public bool FormatToString => _formatToString;

        /// <summary>
        ///     Gets the format string applied when <see cref="FormatToString" /> is <c>true</c>; <c>null</c> when not specified.
        /// </summary>
        public string Format => _format;
    }
}