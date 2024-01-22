// ------------------------------------------------------------
//         File: RPCHandlerBase.cs
//        Brief: RPCHandlerBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 19:59
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal abstract class RPCHandlerBase : IRPCHandler
    {
        public BundlerContexts BundlerContexts { get; set; }
        public abstract string MethodName { get; }
        public abstract Dictionary<string, object> HandleRequest(Dictionary<string, object> args = null);
    }
}