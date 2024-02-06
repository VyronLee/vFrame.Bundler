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
        private readonly ConcurrentQueue<(RequestContext, RespondContext)> _works;
        private bool _started;

        private static readonly JsonObject _emptyRespondJsonData = new JsonObject();

        public SimpleJsonRpcServer(string listenAddress, ILogger logger) {
            if (string.IsNullOrEmpty(listenAddress)) {
                throw new BundleArgumentException("Listen address cannot be null or empty.");
            }
            _logger = logger;
            _handlers = new Dictionary<string, IRpcHandler>();
            _works = new ConcurrentQueue<(RequestContext, RespondContext)>();

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
                HandleRequest(state.Item1, state.Item2);
            }
        }

        private void WaitNextRequest() {
            _listener.BeginGetContext(ListenerCallback, null);
        }

        private void ListenerCallback(IAsyncResult result) {
            var context = _listener.EndGetContext(result);
            var request = context.Request;

            var errorCode = JsonRpcErrorCode.Success;
            var requestData = (JsonObject)null;
            var handler = (IRpcHandler) null;
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

        private void HandleRequest(RequestContext requestContext, RespondContext respondContext) {
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

        private class RequestContext
        {
            public HttpListenerContext HttpContext { get; set; }
            public JsonObject RequestData { get; set; }
            public IRpcHandler Handler { get; set; }
        }
    }
}