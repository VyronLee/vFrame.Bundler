// ------------------------------------------------------------
//         File: QueryHandlersInfoHandler.cs
//        Brief: RPC handler that serializes all registered handlers to a JSON array sorted by creation frame.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:05:43
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// RPC handler that responds to "QueryHandlersInfo" requests with the profile data of every
    /// registered handler context, serialized to JSON and sorted by creation frame.
    /// </summary>
    internal class QueryHandlersInfoHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryHandlersInfo;

        /// <summary>
        /// Collects the JSON representation of each registered handler context, sorted by creation frame.
        /// </summary>
        /// <param name="args">Request arguments (ignored by this handler).</param>
        /// <param name="result">Response object containing a "handlers" array of per-handler JSON data.</param>
        /// <returns>Always <see cref="JsonRpcErrorCode.Success"/>.</returns>
        public override int HandleRequest(JsonObject args, out JsonObject result)
        {
            var handlers = new List<JsonObject>();
            BundlerContexts.ForEachHandler(v => handlers.Add(v.ToJsonData()));
            handlers.Sort(ProfileUtils.SortByCreateFrame);

            result = new JsonObject {
                ["handlers"] = handlers
            };
            return JsonRpcErrorCode.Success;
        }
    }
}