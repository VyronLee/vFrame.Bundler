// ------------------------------------------------------------
//         File: PingPongHandler.cs
//        Brief: Profiler liveness RPC handler; answers PingPong with an empty success response.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 23:56
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    internal class PingPongHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.PingPong;

        public override int HandleRequest(JsonObject args, out JsonObject result)
        {
            result = new JsonObject();
            return JsonRpcErrorCode.Success;
        }
    }
}