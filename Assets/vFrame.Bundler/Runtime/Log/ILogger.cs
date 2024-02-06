//------------------------------------------------------------
//        File:  ILogger.cs
//       Brief:  Bundler logger.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:12
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

namespace vFrame.Bundler
{
    public static class LogLevel
    {
        public const int Debug = 1;
        public const int Info = 2;
        public const int Warning = 3;
        public const int Error = 4;
        public const int Exception = 5;
    }

    public interface ILogger
    {
        void SetLogLevel(int level);
        void LogDebug(string text);
        void LogDebug<T1>(string text, T1 arg1);
        void LogDebug<T1, T2>(string text, T1 arg1, T2 arg2);
        void LogDebug<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        void LogInfo(string text);
        void LogInfo<T1>(string text, T1 arg1);
        void LogInfo<T1, T2>(string text, T1 arg1, T2 arg2);
        void LogInfo<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        void LogWarning(string text);
        void LogWarning<T1>(string text, T1 arg1);
        void LogWarning<T1, T2>(string text, T1 arg1, T2 arg2);
        void LogWarning<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        void LogError(string text);
        void LogError<T1>(string text, T1 arg1);
        void LogError<T1, T2>(string text, T1 arg1, T2 arg2);
        void LogError<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        void LogException(System.Exception exception);
    }
}