// ------------------------------------------------------------
//         File: JsonRpcServer.cs
//        Brief: JsonRpcServer.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-25 20:14
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    public abstract class JsonRpcServer
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();
        public abstract void AddHandler(IRpcHandler handler);

        public static JsonRpcServer CreateSimple(string listenAddress, ILogger logger = null) {
            return new SimpleJsonRpcServer(listenAddress, logger);
        }
    }

    public class RespondContext : IJsonSerializable
    {
        [JsonSerializableProperty]
        public int ErrorCode { get; set; }

        [JsonSerializableProperty]
        public JsonObject RespondData { get; set; }

        public static RespondContext FromJson(JsonObject data) {
            var ret = new RespondContext {
                ErrorCode = (int) data.SafeGetValue<long>("ErrorCode"),
                RespondData = data.SafeGetValue<JsonObject>("RespondData")
            };
            return ret;
        }
    }
}