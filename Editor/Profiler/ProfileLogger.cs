// ------------------------------------------------------------
//         File: ProfileLogger.cs
//        Brief: ProfileLogger.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 19:51
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    internal class ProfileLogger : ILogger
    {
        private int _level = LogLevel.Info;

        public void SetLogLevel(int level) {
            _level = level;
        }

        public void LogDebug(string text) {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(text);
        }

        public void LogDebug<T1>(string text, T1 arg1) {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(string.Format(text, arg1));
        }

        public void LogDebug<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2));
        }

        public void LogDebug<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2, arg3));
        }

        public void LogInfo(string text) {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(text);
        }

        public void LogInfo<T1>(string text, T1 arg1) {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(string.Format(text, arg1));
        }

        public void LogInfo<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2));
        }

        public void LogInfo<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2, arg3));
        }

        public void LogWarning(string text) {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(text);
        }

        public void LogWarning<T1>(string text, T1 arg1) {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(string.Format(text, arg1));
        }

        public void LogWarning<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(string.Format(text, arg1, arg2));
        }

        public void LogWarning<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(string.Format(text, arg1, arg2, arg3));
        }

        public void LogError(string text) {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(text);
        }

        public void LogError<T1>(string text, T1 arg1) {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(string.Format(text, arg1));
        }

        public void LogError<T1, T2>(string text, T1 arg1, T2 arg2) {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(string.Format(text, arg1, arg2));
        }

        public void LogError<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3) {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(string.Format(text, arg1, arg2, arg3));
        }

        public void LogException(System.Exception exception) {
            if (_level > LogLevel.Exception) {
                return;
            }
            Debug.LogException(exception);
        }
    }
}