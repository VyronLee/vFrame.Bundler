// ------------------------------------------------------------
//         File: RPCHandlerBase.cs
//        Brief: RPCHandlerBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 19:59
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal abstract class RPCHandlerBase : IRpcHandler
    {
        public BundlerContexts BundlerContexts { get; set; }
        public abstract string MethodName { get; }
        public abstract int HandleRequest(JsonObject args, out JsonObject result);
    }
}