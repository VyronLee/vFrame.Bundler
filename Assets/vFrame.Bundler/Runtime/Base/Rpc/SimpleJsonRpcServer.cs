// ------------------------------------------------------------
//         File: SimpleJsonRpcServer.cs
//        Brief: HttpListener-based JSON-RPC server: accepts requests async, dispatches to handlers on Update.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:50:53
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace vFrame.Bundler
{
    /// <summary>
    /// <see cref="HttpListener"/>-based JSON-RPC server that accepts requests on background threads,
    /// queues them, and dispatches them to registered handlers on <see cref="Update"/> from the main thread.
    /// </summary>
    internal class SimpleJsonRpcServer : JsonRpcServer
    {
        /// <summary>HTTP listener accepting incoming requests on thread-pool threads.</summary>
        private readonly HttpListener _listener;

        /// <summary>Optional logger; diagnostics are suppressed when null.</summary>
        private readonly ILogger _logger;

        /// <summary>Registered handlers keyed by RPC method name.</summary>
        private readonly Dictionary<string, IRpcHandler> _handlers;

        /// <summary>Requests accepted off-thread, pending main-thread dispatch in <see cref="Update"/>.</summary>
        private readonly ConcurrentQueue<(RequestContext, RespondContext)> _works;

        /// <summary>Indicates whether the listener is currently running.</summary>
        private bool _started;

        /// <summary>Shared empty response payload reused when a handler returns no data.</summary>
        private static readonly JsonObject _emptyRespondJsonData = new JsonObject();

        /// <summary>
        /// Creates an <see cref="HttpListener"/>-based JSON-RPC server.
        /// </summary>
        /// <param name="listenAddress">HTTP URI prefix to listen on, e.g. "http://127.0.0.1:16667/".</param>
        /// <param name="logger">Optional logger; diagnostics are suppressed when null.</param>
        /// <exception cref="BundleArgumentException">Thrown when <paramref name="listenAddress"/> is null or empty.</exception>
        public SimpleJsonRpcServer(string listenAddress, ILogger logger)
        {
            if (string.IsNullOrEmpty(listenAddress)) {
                throw new BundleArgumentException("Listen address cannot be null or empty.");
            }
            _logger = logger;
            _handlers = new Dictionary<string, IRpcHandler>();
            _works = new ConcurrentQueue<(RequestContext, RespondContext)>();

            _listener = new HttpListener();
            _listener.Prefixes.Add(listenAddress);
        }

        /// <summary>Starts the HTTP listener and begins accepting requests; logs a warning on failure.</summary>
        public override void Start()
        {
            try {
                _listener.Start();
                _started = true;
                WaitNextRequest();
            }
            catch (HttpListenerException e) {
                _started = false;
                _logger?.LogWarning("Start HttpListener failed, error code: {0}", e.ErrorCode);
            }
        }

        /// <summary>Stops the listener and clears all registered handlers; no-op when not started.</summary>
        public override void Stop()
        {
            if (!_started) {
                return;
            }
            _started = false;
            _listener.Stop();
            _handlers.Clear();
        }

        /// <summary>Registers a handler under its method name; duplicates are ignored with a warning.</summary>
        /// <param name="handler">Handler to register.</param>
        public override void AddHandler(IRpcHandler handler)
        {
            if (_handlers.ContainsKey(handler.MethodName)) {
                _logger?.LogWarning("Handler with same method name has already been added: {0}", handler.MethodName);
                return;
            }
            _handlers.Add(handler.MethodName, handler);
        }

        /// <summary>Dispatches all requests queued since the last call; call once per frame.</summary>
        public override void Update()
        {
            while (_works.TryDequeue(out var state)) {
                HandleRequest(state.Item1, state.Item2);
            }
        }

        /// <summary>Begins an asynchronous wait for the next incoming request.</summary>
        private void WaitNextRequest()
        {
            _listener.BeginGetContext(ListenerCallback, null);
        }

        /// <summary>
        /// Completes an accepted request: parses its body, resolves its handler, and queues it
        /// for main-thread dispatch; then waits for the next request.
        /// </summary>
        /// <param name="result">Asynchronous result of the pending get-context operation.</param>
        private void ListenerCallback(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;

            var errorCode = JsonRpcErrorCode.Success;
            var requestData = (JsonObject)null;
            var handler = (IRpcHandler)null;
            while (true) {
                using (var streamReader = new StreamReader(request.InputStream, request.ContentEncoding)) {
                    var body = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(body)) {
                        errorCode = JsonRpcErrorCode.InvalidArgs;
                        _logger?.LogDebug("Request body is empty, skip.");
                        break;
                    }

                    requestData = Json.Deserialize(body) as JsonObject;
                    if (null == requestData) {
                        errorCode = JsonRpcErrorCode.InvalidArgs;
                        _logger?.LogDebug("RPCRequestData is null, skip.");
                        break;
                    }

                    var method = (string)requestData["method"];
                    if (!_handlers.TryGetValue(method, out handler)) {
                        errorCode = JsonRpcErrorCode.UnhandledMethod;
                        _logger?.LogWarning("Handler not found for method: {0}, skip.", method);
                        break;
                    }

                    break;
                }
            }

            var requestContext = new RequestContext {
                HttpContext = context,
                RequestData = requestData,
                Handler = handler
            };
            var respondContext = new RespondContext {
                ErrorCode = errorCode
            };
            _works.Enqueue((requestContext, respondContext));

            WaitNextRequest();
        }

        /// <summary>
        /// Invokes the resolved handler on the main thread and writes the JSON response to the HTTP output stream.
        /// </summary>
        /// <param name="requestContext">Accepted request carrying its parsed data and resolved handler.</param>
        /// <param name="respondContext">Response accumulator; may already carry an error code from acceptance.</param>
        private void HandleRequest(RequestContext requestContext, RespondContext respondContext)
        {
            var context = requestContext.HttpContext;
            var requestData = requestContext.RequestData;
            var handler = requestContext.Handler;
            var httpRequest = context.Request;
            var httpRespond = context.Response;

            try {
                if (respondContext.ErrorCode <= 0) {
                    var args = requestData["args"] as JsonObject;
                    respondContext.ErrorCode = handler.HandleRequest(args, out var respondJsonData);
                    respondContext.RespondData = respondJsonData ?? _emptyRespondJsonData;
                }
            }
            catch (System.Exception e) {
                respondContext.ErrorCode = JsonRpcErrorCode.UnknownError;
                _logger.LogException(e);
            }

            var respondJson = respondContext.ToJsonString();
            var buffer = httpRequest.ContentEncoding.GetBytes(respondJson);
            httpRespond.ContentLength64 = buffer.Length;
            httpRespond.OutputStream.Write(buffer, 0, buffer.Length);
            httpRespond.OutputStream.Close();
        }

        /// <summary>State of one accepted request pending main-thread dispatch.</summary>
        private class RequestContext
        {
            /// <summary>HTTP context used to write the response.</summary>
            public HttpListenerContext HttpContext { get; set; }

            /// <summary>Parsed JSON request body; null when the body was empty or not a JSON object.</summary>
            public JsonObject RequestData { get; set; }

            /// <summary>Handler resolved for the requested method; null when no handler matched.</summary>
            public IRpcHandler Handler { get; set; }
        }
    }
}