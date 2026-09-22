// ------------------------------------------------------------
//         File: RPCMethods.cs
//        Brief: Method name constants for the Bundler profiler's JSON-RPC interface.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:11:52
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// String constants of the JSON-RPC method names supported by the Bundler profiler.
    /// </summary>
    public static class RPCMethods
    {
        /// <summary>
        /// Liveness check; the peer echoes the request back.
        /// </summary>
        public const string PingPong = "PingPong";

        /// <summary>
        /// Query the profiler for information about all registered bundle loaders.
        /// </summary>
        public const string QueryLoadersInfo = "QueryLoadersInfo";

        /// <summary>
        /// Query the profiler for information about all running request pipelines.
        /// </summary>
        public const string QueryPipelinesInfo = "QueryPipelinesInfo";

        /// <summary>
        /// Query the profiler for information about a single pipeline, identified by its GUID.
        /// </summary>
        public const string QueryPipelineInfoByGuid = "QueryPipelineInfoByGuid";

        /// <summary>
        /// Query the profiler for information about all registered RPC handlers.
        /// </summary>
        public const string QueryHandlersInfo = "QueryHandlersInfo";

        /// <summary>
        /// Query the profiler for all registered asset links, sorted by create frame.
        /// </summary>
        public const string QueryLinksInfo = "QueryLinksInfo";
    }
}