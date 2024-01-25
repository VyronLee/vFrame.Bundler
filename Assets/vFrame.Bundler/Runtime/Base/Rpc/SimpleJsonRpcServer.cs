// ------------------------------------------------------------
//         File: SimpleJsonRpcServer.cs
//        Brief: SimpleJsonRpcServer.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 16:46
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    internal class SimpleJsonRpcServer : JsonRpcServer
    {
        private readonly HttpListener _listener;
        private readonly ILogger _logger;
        private readonly Dictionary<string, IRpcHandler> _handlers;
        private readonly ConcurrentQueue<RequestContext> _works;
        private bool _started;

        private static readonly JsonObject _emptyRespondJsonData = new JsonObject();

        public SimpleJsonRpcServer(string listenAddress, ILogger logger) {
            if (string.IsNullOrEmpty(listenAddress)) {
                throw new BundleArgumentException("Listen address cannot be null or empty.");
            }
            _logger = logger;
            _handlers = new Dictionary<string, IRpcHandler>();
            _works = new ConcurrentQueue<RequestContext>();

            _listener = new HttpListener();
            _listener.Prefixes.Add(listenAddress);
        }

        public override void Start() {
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

        public override void Stop() {
            if (!_started) {
                return;
            }
            _started = false;
            _listener.Stop();
            _handlers.Clear();
        }

        public override void AddHandler(IRpcHandler handler) {
            if (_handlers.ContainsKey(handler.MethodName)) {
                _logger?.LogWarning("Handler with same method name has already been added: {0}", handler.MethodName);
                return;
            }
            _handlers.Add(handler.MethodName, handler);
        }

        public override void Update() {
            while (_works.TryDequeue(out var state)) {
                HandleRequest(state.Context, state.RequestData, state.Handler);
            }
        }

        private void WaitNextRequest() {
            _listener.BeginGetContext(ListenerCallback, null);
        }

        private void ListenerCallback(IAsyncResult result) {
            var context = _listener.EndGetContext(result);
            var request = context.Request;
            while (true) {
                using (var streamReader = new StreamReader(request.InputStream, request.ContentEncoding)) {
                    var body = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(body)) {
                        _logger?.LogDebug("Request body is empty, skip.");
                        break;
                    }

                    var requestData = Json.Deserialize(body) as JsonObject;
                    if (null == requestData) {
                        _logger?.LogDebug("RPCRequestData is null, skip.");
                        break;
                    }

                    var method = (string)requestData["method"];
                    if (!_handlers.TryGetValue(method, out var handler)) {
                        _logger?.LogWarning("Handler not found for method: {0}, skip.", method);
                        break;
                    }

                    var requestContext = new RequestContext {
                        Context = context,
                        RequestData = requestData,
                        Handler = handler
                    };
                    _works.Enqueue(requestContext);
                    break;
                }
            }
            WaitNextRequest();
        }

        private void HandleRequest(HttpListenerContext context, JsonObject requestData, IRpcHandler handler) {
            var request = context.Request;
            var respond = context.Response;

            var args = requestData["args"] as JsonObject;
            var respondJsonData = handler.HandleRequest(args) ?? _emptyRespondJsonData;
            var respondData = Json.Serialize(respondJsonData);
            var buffer = request.ContentEncoding.GetBytes(respondData);
            respond.ContentLength64 = buffer.Length;
            respond.OutputStream.Write(buffer, 0, buffer.Length);
            respond.OutputStream.Close();
        }

        private class RequestContext
        {
            public HttpListenerContext Context { get; set; }
            public JsonObject RequestData { get; set; }
            public IRpcHandler Handler { get; set; }
        }
    }
}