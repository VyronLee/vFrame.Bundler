// ------------------------------------------------------------
//         File: SimpleJsonRpcClient.cs
//        Brief: HttpWebRequest-based JSON-RPC client: sends POST requests asynchronously and queues responses
//               for dispatch to their callbacks on Update, from the main thread.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:50:48
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     JSON-RPC client that sends requests asynchronously over HTTP and delivers responses to their
    ///     callbacks on the main thread when <see cref="Update"/> runs.
    /// </summary>
    internal class SimpleJsonRpcClient : JsonRpcClient
    {
        private readonly string _address;
        private readonly ILogger _logger;

        /// <summary>Completed requests waiting to be dispatched on the main thread.</summary>
        private readonly ConcurrentQueue<RequestContext> _works;

        /// <summary>
        ///     Create a client targeting the specified JSON-RPC endpoint.
        /// </summary>
        /// <param name="address">HTTP endpoint accepting JSON-RPC POST requests.</param>
        /// <param name="logger">Logger for diagnostics; may be null to disable logging.</param>
        public SimpleJsonRpcClient(string address, ILogger logger)
        {
            _address = address;
            _logger = logger;
            _works = new ConcurrentQueue<RequestContext>();
        }

        /// <summary>
        ///     Dispatch all responses completed since the last call, invoking each request callback on the main thread.
        /// </summary>
        public override void Update()
        {
            while (_works.TryDequeue(out var state)) {
                if (state.RespondData.ErrorCode != JsonRpcErrorCode.Success) {
                    _logger.LogWarning("Send request failed, error code: {0}", state.RespondData.ErrorCode);
                }
                state.Callback?.Invoke(state.RespondData);
            }
        }

        /// <summary>
        ///     Send a request to the endpoint asynchronously via HTTP POST.
        /// </summary>
        /// <param name="method">Name of the remote method to invoke.</param>
        /// <param name="args">Arguments passed to the remote method.</param>
        /// <param name="callback">Invoked with the response on the main thread during <see cref="Update"/>.</param>
        public override void SendRequest(string method, JsonObject args, Action<RespondContext> callback)
        {
            var requestData = new JsonObject {
                { "method", method },
                { "args", args }
            };

            var request = (HttpWebRequest)WebRequest.Create(_address);
            request.Method = "POST";
            request.ContentType = "application/json";

            var state = new RequestContext {
                Request = request,
                RequestData = requestData,
                Callback = callback
            };
            request.BeginGetRequestStream(OnGetRequestStream, state);
        }

        /// <summary>
        ///     Asynchronous callback for the request stream: writes the serialized request body and
        ///     starts waiting for the response.
        /// </summary>
        /// <param name="state">The <see cref="RequestContext"/> passed to <see cref="WebRequest.BeginGetRequestStream"/>.</param>
        private void OnGetRequestStream(IAsyncResult state)
        {
            try {
                var context = (RequestContext)state.AsyncState;
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

        /// <summary>
        ///     Asynchronous callback for the response: deserializes the JSON payload and enqueues the
        ///     context for dispatch on the main thread.
        /// </summary>
        /// <param name="state">The <see cref="RequestContext"/> passed to <see cref="WebRequest.BeginGetResponse"/>.</param>
        private void OnGetResponseStream(IAsyncResult state)
        {
            try {
                var context = (RequestContext)state.AsyncState;
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

        /// <summary>
        ///     State carried across the asynchronous HTTP request/response callbacks.
        /// </summary>
        private class RequestContext
        {
            public HttpWebRequest Request { get; set; }
            public JsonObject RequestData { get; set; }
            public RespondContext RespondData { get; set; }
            public Action<RespondContext> Callback { get; set; }
        }
    }
}