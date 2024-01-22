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

        public override Dictionary<string, object> HandleRequest(Dictionary<string, object> args = null) {
            var loaders = new List<Dictionary<string, object>>();
            BundlerContexts.ForEachLoader(v => loaders.Add(v.ToJsonData()));

            var jsonData = new Dictionary<string, object> {
                ["loaders"] = loaders
            };
            return jsonData;
        }
    }
}