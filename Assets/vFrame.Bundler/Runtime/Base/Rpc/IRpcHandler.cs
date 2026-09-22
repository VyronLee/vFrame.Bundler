// ------------------------------------------------------------
//         File: IRpcHandler.cs
//        Brief: Contract for a JSON-RPC method handler, routed by method name, returning error code and result object.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:45:59
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Contract for a JSON-RPC method handler, routed by method name, returning error code and result object.
    /// </summary>
    public interface IRpcHandler
    {
        /// <summary>Gets the unique method name this handler is registered under.</summary>
        string MethodName { get; }

        /// <summary>Handles a request routed to <see cref="MethodName"/>.</summary>
        /// <param name="args">Request arguments; null when the request carries no arguments object.</param>
        /// <param name="result">Receives the response payload; when null an empty object is sent to the client.</param>
        /// <returns>An error code from <see cref="JsonRpcErrorCode"/>; <see cref="JsonRpcErrorCode.Success"/> on success.</returns>
        int HandleRequest(JsonObject args, out JsonObject result);
    }
}