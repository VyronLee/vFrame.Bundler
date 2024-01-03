// ------------------------------------------------------------
//         File: LogSystem.cs
//        Brief: LogSystem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 15:9
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal class LogSystem : BundlerSystem, ILogger
    {
        private int _level;
        private ILogHandler _logHandler;

        public LogSystem(BundlerContexts bundlerContexts) : base(bundlerContexts) {
            _logHandler = new InternalLogHandler();
        }

        protected override void OnDestroy() {

        }

        public void SetLogLevel(int level) {
            _level = level;
        }

        public void LogDebug(string text) {
            if (_level < LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(text);
        }

        public void LogDebug<T1>(string text, T1 arg1) {
            if (_level < LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(string.Format(text, arg1));
        }

        public void LogDebug<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level < LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(string.Format(text, arg1, arg2));
        }

        public void LogDebug<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level < LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(string.Format(text, arg1, arg2, arg3));
        }

        public void LogInfo(string text) {
            if (_level < LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(text);
        }

        public void LogInfo<T1>(string text, T1 arg1) {
            if (_level < LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(string.Format(text, arg1));
        }

        public void LogInfo<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level < LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(string.Format(text, arg1, arg2));
        }

        public void LogInfo<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level < LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(string.Format(text, arg1, arg2, arg3));
        }

        public void LogWarning(string text) {
            if (_level < LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(text);
        }

        public void LogWarning<T1>(string text, T1 arg1) {
            if (_level < LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(string.Format(text, arg1));
        }

        public void LogWarning<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level < LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(string.Format(text, arg1, arg2));
        }

        public void LogWarning<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level < LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(string.Format(text, arg1, arg2, arg3));
        }

        public void LogError(string text) {
            if (_level < LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(text);
        }

        public void LogError<T1>(string text, T1 arg1) {
            if (_level < LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(string.Format(text, arg1));
        }

        public void LogError<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level < LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(string.Format(text, arg1, arg2));
        }

        public void LogError<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level < LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(string.Format(text, arg1, arg2, arg3));
        }

        public void LogException(System.Exception exception) {
            if (_level < LogLevel.Exception) {
                return;
            }
            _logHandler?.LogException(exception);
        }

        public void SetLogHandler(ILogHandler handler) {
            _logHandler = handler;
        }
    }
}