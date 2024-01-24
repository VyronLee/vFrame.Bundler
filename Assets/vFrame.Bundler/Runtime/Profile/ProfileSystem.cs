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
        private SimpleRPCServer _rpcServer;

        public ProfileSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _rpcServer = new SimpleRPCServer(bundlerContexts, bundlerContexts.Options.ListenAddress);
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
            var handlerTypes = typeof(IRPCHandler).Assembly.GetTypes()
                .Where(v => typeof(IRPCHandler) != v)
                .Where(v => typeof(IRPCHandler).IsAssignableFrom(v))
                .Where(v => !v.IsAbstract);
            foreach (var handlerType in handlerTypes) {
                var handler = Activator.CreateInstance(handlerType) as IRPCHandler;
                if (null == handler) {
                    continue;
                }
                handler.BundlerContexts = BundlerContexts;

                _rpcServer.AddHandler(handler);
            }
        }
    }
}