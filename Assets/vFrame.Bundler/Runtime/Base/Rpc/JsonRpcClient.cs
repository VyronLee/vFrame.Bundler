// ------------------------------------------------------------
//         File: JsonRpcClient.cs
//        Brief: Abstract JSON-RPC client: SendRequest with callback, Update pumps replies; CreateSimple factory.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:46:04
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;

namespace vFrame.Bundler
{
    /// <summary>
    /// Abstract JSON-RPC client that sends method requests and delivers responses to registered callbacks.
    /// Concrete implementations provide the transport; drive reply processing by calling <see cref="Update"/>.
    /// </summary>
    public abstract class JsonRpcClient
    {
        /// <summary>
        /// Pumps pending responses and invokes the callbacks of completed requests. Call once per frame.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Sends a request without arguments and registers <paramref name="callback"/> for its response.
        /// </summary>
        /// <param name="method">Name of the remote method to invoke.</param>
        /// <param name="callback">Invoked with the response context when the reply is processed.</param>
        public void SendRequest(string method, Action<RespondContext> callback)
        {
            SendRequest(method, null, callback);
        }

        /// <summary>
        /// Sends a request with optional arguments and registers <paramref name="callback"/> for its response.
        /// </summary>
        /// <param name="method">Name of the remote method to invoke.</param>
        /// <param name="args">JSON object with the request arguments, or null for none.</param>
        /// <param name="callback">Invoked with the response context when the reply is processed.</param>
        public abstract void SendRequest(string method, JsonObject args, Action<RespondContext> callback);

        /// <summary>
        /// Creates a default HTTP-backed JSON-RPC client posting requests to <paramref name="url"/>.
        /// </summary>
        /// <param name="url">Endpoint URL of the JSON-RPC server.</param>
        /// <param name="logger">Optional logger for diagnostics; may be null.</param>
        /// <returns>A new ready-to-use client instance.</returns>
        public static JsonRpcClient CreateSimple(string url, ILogger logger = null)
        {
            return new SimpleJsonRpcClient(url, logger);
        }
    }
}