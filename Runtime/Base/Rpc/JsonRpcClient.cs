// ------------------------------------------------------------
//         File: JsonRpcClient.cs
//        Brief: JsonRpcClient.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-25 20:14
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;

namespace vFrame.Bundler
{
    public abstract class JsonRpcClient
    {
        public abstract void Update();

        public void SendRequest(string method, Action<JsonObject> callback) {
            SendRequest(method, null, callback);
        }

        public abstract void SendRequest(string method, JsonObject args, Action<JsonObject> callback);

        public static JsonRpcClient CreateSimple(string url, ILogger logger = null) {
            return new SimpleJsonRpcClient(url, logger);
        }
    }
}