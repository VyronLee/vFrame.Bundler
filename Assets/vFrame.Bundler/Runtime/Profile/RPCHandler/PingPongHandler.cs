// ------------------------------------------------------------
//         File: PingPongHandler.cs
//        Brief: Profiler liveness RPC handler; answers PingPong with an empty success response.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:01:52
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Liveness-probe RPC handler that replies to 'PingPong' with an empty success response.
    /// </summary>
    internal class PingPongHandler : RPCHandlerBase
    {
        /// <inheritdoc/>
        public override string MethodName => RPCMethods.PingPong;

        /// <inheritdoc/>
        public override int HandleRequest(JsonObject args, out JsonObject result)
        {
            result = new JsonObject();
            return JsonRpcErrorCode.Success;
        }
    }
}