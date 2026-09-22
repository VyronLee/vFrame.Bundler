// ------------------------------------------------------------
//         File: JsonRpcServer.cs
//        Brief: Abstract JSON-RPC server lifecycle and dispatch contract, plus RespondContext result payload.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:46:12
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Abstract JSON-RPC server providing lifecycle control and handler registration for incoming RPC requests.
    /// </summary>
    public abstract class JsonRpcServer
    {
        /// <summary>Starts the server and begins accepting requests.</summary>
        public abstract void Start();

        /// <summary>Stops the server and releases all registered handlers.</summary>
        public abstract void Stop();

        /// <summary>Dispatches queued requests to their registered handlers; call once per frame.</summary>
        public abstract void Update();

        /// <summary>Registers a handler for the RPC method named by its <see cref="IRpcHandler.MethodName"/>.</summary>
        /// <param name="handler">Handler to register.</param>
        public abstract void AddHandler(IRpcHandler handler);

        /// <summary>Creates an <see cref="System.Net.HttpListener"/>-based server instance.</summary>
        /// <param name="listenAddress">HTTP listen URI prefix, e.g. "http://127.0.0.1:16667/".</param>
        /// <param name="logger">Optional logger; when null, diagnostics are suppressed.</param>
        /// <returns>A new server instance, ready to be started.</returns>
        /// <exception cref="BundleArgumentException">Thrown when <paramref name="listenAddress"/> is null or empty.</exception>
        public static JsonRpcServer CreateSimple(string listenAddress, ILogger logger = null)
        {
            return new SimpleJsonRpcServer(listenAddress, logger);
        }
    }

    /// <summary>
    /// RPC response payload returned to the caller, carrying an error code and JSON-serialized result data.
    /// </summary>
    public class RespondContext : IJsonSerializable
    {
        /// <summary>Error code reported to the caller; non-positive values are treated as success (see <see cref="JsonRpcErrorCode"/>).</summary>
        [JsonSerializableProperty]
        public int ErrorCode { get; set; }

        /// <summary>Result data written into the response; may be null when the request fails before handler dispatch.</summary>
        [JsonSerializableProperty]
        public JsonObject RespondData { get; set; }

        /// <summary>Deserializes a response context from its JSON representation.</summary>
        /// <param name="data">JSON object containing "ErrorCode" and "RespondData" fields.</param>
        /// <returns>The deserialized response context.</returns>
        public static RespondContext FromJson(JsonObject data)
        {
            var ret = new RespondContext {
                ErrorCode = (int)data.SafeGetValue<long>("ErrorCode"),
                RespondData = data.SafeGetValue<JsonObject>("RespondData")
            };
            return ret;
        }
    }
}