// ------------------------------------------------------------
//         File: QueryHandlersInfoHandler.cs
//        Brief: QueryHandlersInfoHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-4 20:3
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class QueryHandlersInfoHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryHandlersInfo;

        public override int HandleRequest(JsonObject args, out JsonObject result) {
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