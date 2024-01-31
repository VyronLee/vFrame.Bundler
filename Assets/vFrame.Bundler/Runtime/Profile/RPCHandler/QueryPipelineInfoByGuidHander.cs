// ------------------------------------------------------------
//         File: QueryPipelineInfoByGuidHander.cs
//        Brief: QueryPipelineInfoByGuidHander.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-30 22:18
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal class QueryPipelineInfoByGuidHander : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.QueryPipelineInfoByGuid;

        public override int HandleRequest(JsonObject args, out JsonObject result) {
            result = new JsonObject();

            var guid = args.SafeGetValue<string>("guid");
            if (string.IsNullOrEmpty(guid)) {
                return JsonRpcErrorCode.InvalidArgs;
            }

            if (!BundlerContexts.TryGetPipeline(guid, out var pipeline)) {
                return JsonRpcErrorCode.PipelineNotExist;
            }
            result["loaders"] = pipeline.Loaders;
            return JsonRpcErrorCode.Success;
        }
    }
}