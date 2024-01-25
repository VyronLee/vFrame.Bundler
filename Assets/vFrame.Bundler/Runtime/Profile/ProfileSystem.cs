// ------------------------------------------------------------
//         File: ProfileSystem.cs
//        Brief: ProfileSystem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 16:44
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Linq;

namespace vFrame.Bundler
{
    internal class ProfileSystem : BundlerSystem
    {
        private JsonRpcServer _rpcServer;

        public ProfileSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _rpcServer = JsonRpcServer.CreateSimple(
                bundlerContexts.Options.ListenAddress,
                bundlerContexts.Bundler.GetSystem<LogSystem>());
            _rpcServer.Start();

            CreateRPCHandlers();
        }

        protected override void OnDestroy() {
            _rpcServer.Stop();
            _rpcServer = null;
        }

        protected override void OnUpdate() {
            _rpcServer.Update();
        }

        private void CreateRPCHandlers() {
            var handlerTypes = typeof(RPCHandlerBase).Assembly.GetTypes()
                .Where(v => typeof(RPCHandlerBase) != v)
                .Where(v => typeof(RPCHandlerBase).IsAssignableFrom(v))
                .Where(v => !v.IsAbstract);
            foreach (var handlerType in handlerTypes) {
                var handler = Activator.CreateInstance(handlerType) as RPCHandlerBase;
                if (null == handler) {
                    continue;
                }
                handler.BundlerContexts = BundlerContexts;

                _rpcServer.AddHandler(handler);
            }
        }
    }
}