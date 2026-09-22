// ------------------------------------------------------------
//         File: ProfileUtils.cs
//        Brief: Sorting helpers for profiler JSON entries, comparing them by their CreateFrame field.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:01:48
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Shared helper routines for profiler data processing.
    /// </summary>
    internal static class ProfileUtils
    {
        /// <summary>
        /// Compares two profiler JSON objects by their "CreateFrame" field value.
        /// </summary>
        /// <param name="a">The first entry to compare.</param>
        /// <param name="b">The second entry to compare.</param>
        /// <returns>A negative value, zero, or a positive value if <paramref name="a"/>'s create frame is
        /// earlier than, equal to, or later than <paramref name="b"/>'s, respectively.</returns>
        public static int SortByCreateFrame(JsonObject a, JsonObject b)
        {
            var createFrameA = a.SafeGetValue<int>("CreateFrame");
            var createFrameB = b.SafeGetValue<int>("CreateFrame");
            return createFrameA.CompareTo(createFrameB);
        }
    }
}