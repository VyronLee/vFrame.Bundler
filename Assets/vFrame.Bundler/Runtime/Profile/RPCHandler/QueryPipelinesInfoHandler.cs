// ------------------------------------------------------------
//         File: QueryPipelinesInfoHandler.cs
//        Brief: QueryPipelinesInfoHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 22:52
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class QueryPipelinesInfoHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryPipelinesInfo;

        public override int HandleRequest(JsonObject args, out JsonObject result) {
            var pipelines = new List<Dictionary<string, object>>();
            BundlerContexts.ForEachPipeline(v => pipelines.Add(v.ToJsonData()));

            result = new JsonObject {
                ["pipelines"] = pipelines
            };
            return JsonRpcErrorCode.Success;
        }
    }
}