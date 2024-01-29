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

        public override JsonObject HandleRequest(JsonObject args = null) {
            var pipelines = new List<Dictionary<string, object>>();
            BundlerContexts.ForEachPipeline(v => pipelines.Add(v.ToJsonData()));

            var jsonData = new JsonObject {
                ["pipelines"] = pipelines
            };
            return jsonData;
        }
    }
}