// ------------------------------------------------------------
//         File: SimpleRPCClient.cs
//        Brief: SimpleRPCClient.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 21:35
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

namespace vFrame.Bundler.Profiler
{
    internal class SimpleRPCClient
    {
        private readonly string _address;
        private const int ConnectionTimeout = 2000;

        public SimpleRPCClient(string address) {
            _address = address;
        }

        public bool Ping() {
            var resetEvent = new ManualResetEventSlim(false);
            SendRequest(RPCMethods.PingPong, ResetOnPing);
            return resetEvent.Wait(ConnectionTimeout);

            void ResetOnPing(Dictionary<string, object> jsonData) {
                if (!jsonData.TryGetValue("success", out var success)) {
                    return;
                }
                if ((bool)success) {
                    resetEvent.Set();
                }
            }
        }

        public void SendRequest(string method, Action<Dictionary<string, object>> callback) {
            SendRequest(method, null, callback);
        }

        public void SendRequest(string method, Dictionary<string, object> args, Action<Dictionary<string, object>> callback) {
            var requestData = new Dictionary<string, object> {
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
                        if (string.IsNullOrEmpty(responseData)) {
                            return;
                        }
                        Debug.Log($"OnResponse: {responseData}");
                        var jsonData = Json.Deserialize(responseData) as Dictionary<string, object>;
                        context.Callback?.Invoke(jsonData);
                    }
                }
            }
            catch (WebException e) {
                Debug.LogWarning(e.Message);
            }
        }

        private class RequestContext
        {
            public HttpWebRequest Request { get; set; }
            public Dictionary<string, object> RequestData { get; set; }
            public Action<Dictionary<string, object>> Callback { get; set; }
        }
    }
}