// ------------------------------------------------------------
//         File: InternalLogHandler.cs
//        Brief: Default log handler that forwards every bundler log call to the matching Unity Debug output.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:14:43
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Default <see cref="ILogHandler"/> implementation that forwards every log call to the
    /// corresponding Unity <c>Debug</c> output (Log/LogWarning/LogError/LogException).
    /// </summary>
    internal class InternalLogHandler : ILogHandler
    {
        /// <summary>Forwards a debug message to <see cref="Debug.Log(object)"/>.</summary>
        /// <param name="text">Message text to log.</param>
        public void LogDebug(string text)
        {
            Debug.Log(text);
        }

        /// <summary>Forwards an informational message to <see cref="Debug.Log(object)"/>.</summary>
        /// <param name="text">Message text to log.</param>
        public void LogInfo(string text)
        {
            Debug.Log(text);
        }

        /// <summary>Forwards a warning message to <see cref="Debug.LogWarning(object)"/>.</summary>
        /// <param name="text">Message text to log.</param>
        public void LogWarning(string text)
        {
            Debug.LogWarning(text);
        }

        /// <summary>Forwards an error message to <see cref="Debug.LogError(object)"/>.</summary>
        /// <param name="text">Message text to log.</param>
        public void LogError(string text)
        {
            Debug.LogError(text);
        }

        /// <summary>Forwards an exception to <see cref="Debug.LogException"/>.</summary>
        /// <param name="exception">Exception to log.</param>
        public void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}