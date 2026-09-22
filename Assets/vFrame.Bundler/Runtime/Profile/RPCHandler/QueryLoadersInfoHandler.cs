// ------------------------------------------------------------
//         File: QueryLoadersInfoHandler.cs
//        Brief: RPC handler that serializes all active loaders to a JSON array sorted by creation frame.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:05:52
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// RPC handler that responds to "QueryLoadersInfo" requests with the profile data of every
    /// active loader context, serialized to JSON and sorted by creation frame.
    /// </summary>
    internal class QueryLoadersInfoHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryLoadersInfo;

        /// <summary>
        /// Collects the JSON representation of each active loader context, sorted by creation frame.
        /// </summary>
        /// <param name="args">Request arguments (ignored by this handler).</param>
        /// <param name="result">Response object containing a "loaders" array of per-loader JSON data.</param>
        /// <returns>Always <see cref="JsonRpcErrorCode.Success"/>.</returns>
        public override int HandleRequest(JsonObject args, out JsonObject result)
        {
            var loaders = new List<JsonObject>();
            BundlerContexts.ForEachLoader(v => loaders.Add(v.ToJsonData()));
            loaders.Sort(ProfileUtils.SortByCreateFrame);

            result = new JsonObject {
                ["loaders"] = loaders
            };
            return JsonRpcErrorCode.Success;
        }
    }
}