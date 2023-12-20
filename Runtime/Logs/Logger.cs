//------------------------------------------------------------
//        File:  Logger.cs
//       Brief:  Bundler logger.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:12
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using System;
using UnityEngine;

namespace vFrame.Bundler.Logs
{
    public static class Logger
    {
        private static int _level = LogLevel.ERROR;

        public static void SetLogLevel(int level) {
            if (level < LogLevel.LOG_LEVEL_MIN || level > LogLevel.LOG_LEVEL_MAX)
                throw new ArgumentException("Invalid log level: " + level);

            _level = level;
        }

        private static void Log(string text, params object[] args) {
            Debug.Log(string.Format(text, args));
        }

        public static void LogVerbose(string text, params object[] args) {
            if (_level > LogLevel.VERBOSE)
                return;

            Log(text, args);
        }

        public static void LogInfo(string text, params object[] args) {
            if (_level > LogLevel.INFO)
                return;

            Log(text, args);
        }

        public static void LogError(string text, params object[] args) {
            if (_level > LogLevel.ERROR)
                return;

            Debug.LogError(string.Format(text, args));
        }

        public static class LogLevel
        {
            internal const int LOG_LEVEL_MIN = 1;
            public const int VERBOSE = 1;
            public const int INFO = 2;
            public const int ERROR = 3;
            internal const int LOG_LEVEL_MAX = 3;
        }
    }
}