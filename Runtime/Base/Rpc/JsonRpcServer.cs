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
}