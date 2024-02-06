// ------------------------------------------------------------
//         File: QueryLinksInfoHandler.cs
//        Brief: QueryLinksInfoHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-5 17:41
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class QueryLinksInfoHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryLinksInfo;

        public override int HandleRequest(JsonObject args, out JsonObject result) {
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