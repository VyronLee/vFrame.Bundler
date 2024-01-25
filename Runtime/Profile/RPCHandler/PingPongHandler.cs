// ------------------------------------------------------------
//         File: PingPongHandler.cs
//        Brief: PingPongHandler.cs
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

        public override JsonObject HandleRequest(JsonObject args = null) {
            var jsonData = new JsonObject {
                { "success", true }
            };
            return jsonData;
        }
    }
}