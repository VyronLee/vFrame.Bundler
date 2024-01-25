// ------------------------------------------------------------
//         File: IRpcHandler.cs
//        Brief: IRpcHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 17:12
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    public interface IRpcHandler
    {
        string MethodName { get; }
        JsonObject HandleRequest(JsonObject args = null);
    }
}