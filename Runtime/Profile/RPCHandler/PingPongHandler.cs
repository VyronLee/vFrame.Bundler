// ------------------------------------------------------------
//         File: PingPongHandler.cs
//        Brief: PingPongHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 23:56
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class PingPongHandler : RPCHandlerBase
    {
        public override string MethodName => RPCMethods.PingPong;

        public override Dictionary<string, object> HandleRequest(Dictionary<string, object> args = null) {
            var jsonData = new Dictionary<string, object> {
                { "success", true }
            };
            return jsonData;
        }
    }
}