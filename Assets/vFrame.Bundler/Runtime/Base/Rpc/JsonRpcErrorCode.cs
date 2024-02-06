// ------------------------------------------------------------
//         File: JsonRpcErrorCode.cs
//        Brief: JsonRpcErrorCode.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 20:15
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    public static class JsonRpcErrorCode
    {
        public const int Success = 0;
        public const int UnknownError = 1;
        public const int InvalidArgs = 2;
        public const int UnhandledMethod = 3;
    }
}