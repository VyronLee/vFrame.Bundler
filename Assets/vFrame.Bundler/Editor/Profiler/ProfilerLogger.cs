// ------------------------------------------------------------
//         File: ProfilerLogger.cs
//        Brief: Level-filtered ILogger implementation that writes profiler messages to the Unity console.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:18:28
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Level-filtered <see cref="ILogger"/> implementation that writes profiler messages to the Unity console.
    /// </summary>
    internal class ProfilerLogger : ILogger
    {
        /// <summary>
        ///     Current severity threshold; messages less severe than this level are suppressed.
        /// </summary>
        private int _level;

        /// <summary>
        ///     Creates the logger with an initial severity threshold.
        /// </summary>
        /// <param name="level">Initial severity threshold (see <see cref="LogLevel"/>).</param>
        public ProfilerLogger(int level = LogLevel.Info)
        {
            _level = level;
        }

        /// <summary>
        ///     Sets the severity threshold; messages less severe than this level are suppressed.
        /// </summary>
        /// <param name="level">New severity threshold (see <see cref="LogLevel"/>).</param>
        public void SetLogLevel(int level)
        {
            _level = level;
        }

        /// <summary>
        ///     Writes a debug-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Message text.</param>
        public void LogDebug(string text)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(text);
        }

        /// <summary>
        ///     Writes a formatted debug-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogDebug<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(string.Format(text, arg1));
        }

        /// <summary>
        ///     Writes a formatted debug-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogDebug<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2));
        }

        /// <summary>
        ///     Writes a formatted debug-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogDebug<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        ///     Writes an info-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Message text.</param>
        public void LogInfo(string text)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(text);
        }

        /// <summary>
        ///     Writes a formatted info-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogInfo<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(string.Format(text, arg1));
        }

        /// <summary>
        ///     Writes a formatted info-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogInfo<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2));
        }

        /// <summary>
        ///     Writes a formatted info-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogInfo<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            Debug.Log(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        ///     Writes a warning-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Message text.</param>
        public void LogWarning(string text)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(text);
        }

        /// <summary>
        ///     Writes a formatted warning-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogWarning<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(string.Format(text, arg1));
        }

        /// <summary>
        ///     Writes a formatted warning-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogWarning<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(string.Format(text, arg1, arg2));
        }

        /// <summary>
        ///     Writes a formatted warning-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogWarning<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            Debug.LogWarning(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        ///     Writes an error-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Message text.</param>
        public void LogError(string text)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(text);
        }

        /// <summary>
        ///     Writes a formatted error-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogError<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(string.Format(text, arg1));
        }

        /// <summary>
        ///     Writes a formatted error-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogError<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(string.Format(text, arg1, arg2));
        }

        /// <summary>
        ///     Writes a formatted error-level message to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogError<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            Debug.LogError(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        ///     Writes an exception to the Unity console when the level threshold allows it.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        public void LogException(System.Exception exception)
        {
            if (_level > LogLevel.Exception) {
                return;
            }
            Debug.LogException(exception);
        }
    }
}