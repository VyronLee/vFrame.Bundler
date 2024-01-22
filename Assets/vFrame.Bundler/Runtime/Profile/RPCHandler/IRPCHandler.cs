// ------------------------------------------------------------
//         File: IRPCHandler.cs
//        Brief: IRPCHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 17:12
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal interface IRPCHandler
    {
        BundlerContexts BundlerContexts { get; set; }
        string MethodName { get; }
        Dictionary<string, object> HandleRequest(Dictionary<string, object> args = null);
    }
}