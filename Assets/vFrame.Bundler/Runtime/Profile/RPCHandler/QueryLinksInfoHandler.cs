// ------------------------------------------------------------
//         File: QueryLinksInfoHandler.cs
//        Brief: RPC handler that replies with all asset links serialized to JSON, sorted by create frame.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:05:47
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     RPC handler for <c>QueryLinksInfo</c> requests. Replies with every registered asset link
    ///     serialized to JSON, sorted by create frame order.
    /// </summary>
    internal class QueryLinksInfoHandler : RPCHandlerBase
    {
        /// <summary>Gets the RPC method name this handler serves.</summary>
        public override string MethodName => RPCMethods.QueryLinksInfo;

        /// <summary>
        ///     Collects all asset links and builds the JSON-RPC response.
        /// </summary>
        /// <param name="args">Request arguments; ignored by this handler.</param>
        /// <param name="result">Response payload containing a <c>links</c> array of serialized link objects.</param>
        /// <returns>The JSON-RPC status code; always <see cref="JsonRpcErrorCode.Success" /> on return.</returns>
        public override int HandleRequest(JsonObject args, out JsonObject result)
        {
            var links = new List<JsonObject>();
            BundlerContexts.ForEachLinks((obj, link) => links.Add(link.ToJsonData()));
            links.Sort(ProfileUtils.SortByCreateFrame);

            result = new JsonObject {
                ["links"] = links
            };
            return JsonRpcErrorCode.Success;
        }
    }
}