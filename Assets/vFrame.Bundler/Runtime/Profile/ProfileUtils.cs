// ------------------------------------------------------------
//         File: ProfileUtils.cs
//        Brief: ProfileUtils.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-4 20:42
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal static class ProfileUtils
    {
        public static int SortByCreateFrame(JsonObject a, JsonObject b) {
            var createFrameA = a.SafeGetValue<int>("CreateFrame");
            var createFrameB = b.SafeGetValue<int>("CreateFrame");
            return createFrameA.CompareTo(createFrameB);
        }
    }
}