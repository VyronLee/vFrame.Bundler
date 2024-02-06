// ------------------------------------------------------------
//         File: InternalLogHandler.cs
//        Brief: InternalLogHandler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 15:9
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    internal class InternalLogHandler : ILogHandler
    {
        public void LogDebug(string text) {
            Debug.Log(text);
        }

        public void LogInfo(string text) {
            Debug.Log(text);
        }

        public void LogWarning(string text) {
            Debug.LogWarning(text);
        }

        public void LogError(string text) {
            Debug.LogError(text);
        }

        public void LogException(System.Exception exception) {
            Debug.LogException(exception);
        }
    }
}