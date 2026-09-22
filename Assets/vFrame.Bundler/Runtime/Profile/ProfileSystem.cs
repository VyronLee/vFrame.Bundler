// ------------------------------------------------------------
//         File: ProfileSystem.cs
//        Brief: Bundler subsystem that hosts the JSON-RPC profiler server and auto-registers all RPCHandlerBase
//               implementations found via reflection.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:01:43
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Linq;

namespace vFrame.Bundler
{
    /// <summary>
    /// Bundler subsystem that hosts the JSON-RPC profiler server and auto-registers all concrete
    /// <see cref="RPCHandlerBase"/> implementations found in the assembly via reflection.
    /// </summary>
    internal class ProfileSystem : BundlerSystem
    {
        /// <summary>
        /// JSON-RPC server serving profiler requests; <see langword="null"/> when startup failed or after teardown.
        /// </summary>
        private JsonRpcServer _rpcServer;

        /// <summary>
        /// Creates and starts the JSON-RPC server on the configured listen address, then registers all RPC handlers.
        /// Startup failures are logged as a warning and leave the subsystem inert.
        /// </summary>
        /// <param name="bundlerContexts">Contexts shared across the owning <see cref="Bundler"/>.</param>
        public ProfileSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {
            try {
                _rpcServer = JsonRpcServer.CreateSimple(
                    bundlerContexts.Options.ListenAddress,
                    bundlerContexts.Bundler.GetSystem<LogSystem>());
                _rpcServer.Start();
            }
            catch (Exception e) {
                Facade.GetSystem<LogSystem>().LogWarning("Start RPC server failed: {0}", e.Message);
                _rpcServer?.Stop();
                _rpcServer = null;
                return;
            }

            CreateRPCHandlers();
        }

        /// <summary>
        /// Stops and releases the JSON-RPC server.
        /// </summary>
        protected override void OnDestroy()
        {
            _rpcServer?.Stop();
            _rpcServer = null;
        }

        /// <summary>
        /// Pumps the JSON-RPC server once per frame; no-op when startup failed.
        /// </summary>
        protected override void OnUpdate()
        {
            _rpcServer?.Update();
        }

        /// <summary>
        /// Instantiates every concrete <see cref="RPCHandlerBase"/> derived type in the assembly, injects the
        /// shared <see cref="BundlerContexts"/> into each, and registers them with the JSON-RPC server.
        /// </summary>
        private void CreateRPCHandlers()
        {
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