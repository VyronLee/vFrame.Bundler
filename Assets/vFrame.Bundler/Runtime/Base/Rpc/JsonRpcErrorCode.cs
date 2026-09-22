// ------------------------------------------------------------
//         File: JsonRpcErrorCode.cs
//        Brief: JSON-RPC response error code constants used by the RPC dispatcher.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:46:08
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// JSON-RPC response error code constants used by the RPC dispatcher.
    /// </summary>
    public static class JsonRpcErrorCode
    {
        /// <summary>Request handled successfully (code 0).</summary>
        public const int Success = 0;

        /// <summary>Unspecified failure with no more specific code available (code 1).</summary>
        public const int UnknownError = 1;

        /// <summary>Request arguments failed validation (code 2).</summary>
        public const int InvalidArgs = 2;

        /// <summary>No handler is registered for the requested method (code 3).</summary>
        public const int UnhandledMethod = 3;
    }
}