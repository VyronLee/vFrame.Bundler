// ------------------------------------------------------------
//         File: QueryPipelinesInfoHandler.cs
//        Brief: RPC handler that replies with all loader pipelines serialized to JSON, sorted by create frame.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:05:56
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     RPC handler for <c>QueryPipelinesInfo</c> requests. Replies with every registered loader
    ///     pipeline serialized to JSON, sorted by create frame order.
    /// </summary>
    internal class QueryPipelinesInfoHandler : RPCHandlerBase
    {
        /// <summary>Gets the RPC method name this handler serves.</summary>
        public override string MethodName => RPCMethods.QueryPipelinesInfo;

        /// <summary>
        ///     Collects all loader pipelines and builds the JSON-RPC response.
        /// </summary>
        /// <param name="args">Request arguments; ignored by this handler.</param>
        /// <param name="result">Response payload containing a <c>pipelines</c> array of serialized pipeline objects.</param>
        /// <returns>The JSON-RPC status code; always <see cref="JsonRpcErrorCode.Success" /> on return.</returns>
        public override int HandleRequest(JsonObject args, out JsonObject result)
        {
            var pipelines = new List<JsonObject>();
            BundlerContexts.ForEachPipeline(v => pipelines.Add(v.ToJsonData()));
            pipelines.Sort(ProfileUtils.SortByCreateFrame);

            result = new JsonObject {
                ["pipelines"] = pipelines
            };
            return JsonRpcErrorCode.Success;
        }
    }
}