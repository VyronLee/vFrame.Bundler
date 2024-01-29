// ------------------------------------------------------------
//         File: SimpleJsonRpcClient.cs
//        Brief: SimpleJsonRpcClient.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 21:35
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using UnityEngine;

namespace vFrame.Bundler
{
    internal class SimpleJsonRpcClient : JsonRpcClient
    {
        private readonly string _address;
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<RequestContext> _works;

        public SimpleJsonRpcClient(string address, ILogger logger) {
            _address = address;
            _logger = logger;
            _works = new ConcurrentQueue<RequestContext>();
        }

        public override void Update() {
            while (_works.TryDequeue(out var state)) {
                if (state.RespondData.ErrorCode != JsonRpcErrorCode.Success) {
                    _logger.LogWarning("Send request failed, error code: {0}", state.RespondData.ErrorCode);
                }
                state.Callback?.Invoke(state.RespondData);
            }
        }

        public override void SendRequest(string method, JsonObject args, Action<RespondContext> callback) {
            var requestData = new JsonObject {
                { "method", method },
                { "args", args }
            };

            var request = (HttpWebRequest) WebRequest.Create(_address);
            request.Method = "POST";
            request.ContentType = "application/json";

            var state = new RequestContext {
                Request = request,
                RequestData = requestData,
                Callback = callback
            };
            request.BeginGetRequestStream(OnGetRequestStream, state);
        }

        private void OnGetRequestStream(IAsyncResult state) {
            try {
                var context =  (RequestContext) state.AsyncState;
                using (var streamWriter = new StreamWriter(context.Request.EndGetRequestStream(state))) {
                    streamWriter.Write(Json.Serialize(context.RequestData));
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                context.Request.BeginGetResponse(OnGetResponseStream, context);
            }
            catch (WebException e) {
                Debug.LogWarning(e.Message);
            }
        }

        private void OnGetResponseStream(IAsyncResult state) {
            try {
                var context =  (RequestContext) state.AsyncState;
                using (var respond = context.Request.EndGetResponse(state)) {
                    var respondStream = respond.GetResponseStream();
                    if (null == respondStream) {
                        return;
                    }
                    using (var streamReader = new StreamReader(respondStream)) {
                        var responseData = streamReader.ReadToEnd();
                        _logger?.LogInfo($"RPC: {context.RequestData["method"]}, respond: {responseData}");
                        if (string.IsNullOrEmpty(responseData)) {
                            return;
                        }

                        var jsonData = Json.Deserialize(responseData) as JsonObject;
                        if (null == jsonData) {
                            return;
                        }
                        context.RespondData = RespondContext.FromJson(jsonData);

                        _works.Enqueue(context);
                    }
                }
            }
            catch (WebException e) {
                _logger?.LogWarning(e.Message);
            }
        }

        private class RequestContext
        {
            public HttpWebRequest Request { get; set; }
            public JsonObject RequestData { get; set; }
            public RespondContext RespondData { get; set; }
            public Action<RespondContext> Callback { get; set; }
        }
    }
}