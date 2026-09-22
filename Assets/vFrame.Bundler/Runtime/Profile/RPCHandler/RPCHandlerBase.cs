// ------------------------------------------------------------
//         File: RPCHandlerBase.cs
//        Brief: Base class for JSON-RPC method handlers, exposing the owning BundlerContexts to derived handlers.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:06:00
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Base class for JSON-RPC method handlers, exposing the owning <see cref="BundlerContexts"/> to derived handlers.
    /// </summary>
    internal abstract class RPCHandlerBase : IRpcHandler
    {
        /// <summary>The bundler contexts this handler operates on, injected by the RPC dispatcher.</summary>
        public BundlerContexts BundlerContexts { get; set; }

        /// <inheritdoc cref="IRpcHandler.MethodName"/>
        public abstract string MethodName { get; }

        /// <inheritdoc cref="IRpcHandler.HandleRequest"/>
        public abstract int HandleRequest(JsonObject args, out JsonObject result);
    }
}