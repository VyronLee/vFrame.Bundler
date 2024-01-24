// ------------------------------------------------------------
//         File: SimpleRPCServer.cs
//        Brief: SimpleRPCServer.cs
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
    internal class SimpleRPCServer
    {
        private readonly HttpListener _listener;
        private readonly BundlerContexts _bundlerContexts;
        private readonly Dictionary<string, IRPCHandler> _handlers;
        private readonly ConcurrentQueue<RequestContext> _works;
        private bool _started;

        private static readonly Dictionary<string, object> _emptyRespondJsonData = new Dictionary<string, object>();

        public SimpleRPCServer(BundlerContexts contexts, string listenAddress) {
            if (string.IsNullOrEmpty(listenAddress)) {
                throw new BundleArgumentException("Listen address cannot be null or empty.");
            }
            _bundlerContexts = contexts;
            _handlers = new Dictionary<string, IRPCHandler>();
            _works = new ConcurrentQueue<RequestContext>();

            _listener = new HttpListener();
            _listener.Prefixes.Add(listenAddress);
        }

        public void Start() {
            try {
                _listener.Start();
                _started = true;
                WaitNextRequest();
            }
            catch (HttpListenerException e) {
                _started = false;
                GetLogSystem().LogWarning("Start HttpListener failed, error code: {0}", e.ErrorCode);
            }
        }

        public void Stop() {
            if (!_started) {
                return;
            }
            _started = false;
            _listener.Stop();
            _handlers.Clear();
        }

        public void AddHandler(IRPCHandler handler) {
            if (_handlers.ContainsKey(handler.MethodName)) {
                GetLogSystem().LogWarning("Handler with same method name has already been added: {0}", handler.MethodName);
                return;
            }
            _handlers.Add(handler.MethodName, handler);
        }

        private LogSystem GetLogSystem() {
            return _bundlerContexts.Bundler.GetSystem<LogSystem>();
        }

        public void Update() {
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
                        GetLogSystem().LogDebug("Request body is empty, skip.");
                        break;
                    }

                    var requestData = Json.Deserialize(body) as Dictionary<string, object>;
                    if (null == requestData) {
                        GetLogSystem().LogDebug("RPCRequestData is null, skip.");
                        break;
                    }

                    var method = (string)requestData["method"];
                    if (!_handlers.TryGetValue(method, out var handler)) {
                        GetLogSystem().LogWarning("Handler not found for method: {0}, skip.", method);
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

        private void HandleRequest(HttpListenerContext context, Dictionary<string, object> requestData, IRPCHandler handler) {
            var request = context.Request;
            var respond = context.Response;

            var args = requestData["args"] as Dictionary<string, object>;
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
            public Dictionary<string, object> RequestData { get; set; }
            public IRPCHandler Handler { get; set; }
        }
    }
}