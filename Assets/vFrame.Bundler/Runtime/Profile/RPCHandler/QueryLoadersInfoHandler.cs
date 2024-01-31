// ------------------------------------------------------------
//         File: QueryLoadersInfoHandler.cs
//        Brief: QueryLoadersInfoHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 19:57
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class QueryLoadersInfoHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryLoadersInfo;

        public override int HandleRequest(JsonObject args, out JsonObject result) {
            var loaders = new List<Dictionary<string, object>>();
            BundlerContexts.ForEachLoader(v => loaders.Add(v.ToJsonData()));

            result = new JsonObject {
                ["loaders"] = loaders
            };
            return JsonRpcErrorCode.Success;
        }
    }
}