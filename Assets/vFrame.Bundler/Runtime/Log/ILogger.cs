// ------------------------------------------------------------
//         File: ILogger.cs
//        Brief: Bundler logging contract: severity levels and leveled log methods with composite-format arguments.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:14:35
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Integer severity levels for bundler logging, ordered from least to most severe.
    /// </summary>
    public static class LogLevel
    {
        public const int Debug = 1;
        public const int Info = 2;
        public const int Warning = 3;
        public const int Error = 4;
        public const int Exception = 5;
    }

    /// <summary>
    /// Leveled logging contract for bundler systems; messages below the configured
    /// level are dropped. Format overloads expand arguments via <see cref="string.Format"/>.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Sets the minimum severity a message must have to be emitted.
        /// </summary>
        /// <param name="level">Minimum severity, one of the <see cref="LogLevel"/> constants.</param>
        void SetLogLevel(int level);

        /// <summary>Writes a debug-level message.</summary>
        /// <param name="text">Message text.</param>
        void LogDebug(string text);

        /// <summary>Writes a debug-level message with one formatted argument.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        void LogDebug<T1>(string text, T1 arg1);

        /// <summary>Writes a debug-level message with two formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        void LogDebug<T1, T2>(string text, T1 arg1, T2 arg2);

        /// <summary>Writes a debug-level message with three formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        void LogDebug<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>Writes an info-level message.</summary>
        /// <param name="text">Message text.</param>
        void LogInfo(string text);

        /// <summary>Writes an info-level message with one formatted argument.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        void LogInfo<T1>(string text, T1 arg1);

        /// <summary>Writes an info-level message with two formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        void LogInfo<T1, T2>(string text, T1 arg1, T2 arg2);

        /// <summary>Writes an info-level message with three formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        void LogInfo<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>Writes a warning-level message.</summary>
        /// <param name="text">Message text.</param>
        void LogWarning(string text);

        /// <summary>Writes a warning-level message with one formatted argument.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        void LogWarning<T1>(string text, T1 arg1);

        /// <summary>Writes a warning-level message with two formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        void LogWarning<T1, T2>(string text, T1 arg1, T2 arg2);

        /// <summary>Writes a warning-level message with three formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        void LogWarning<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>Writes an error-level message.</summary>
        /// <param name="text">Message text.</param>
        void LogError(string text);

        /// <summary>Writes an error-level message with one formatted argument.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        void LogError<T1>(string text, T1 arg1);

        /// <summary>Writes an error-level message with two formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        void LogError<T1, T2>(string text, T1 arg1, T2 arg2);

        /// <summary>Writes an error-level message with three formatted arguments.</summary>
        /// <param name="text">Composite format string.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        void LogError<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>Writes an exception report, regardless of the configured level.</summary>
        /// <param name="exception">Exception to report.</param>
        void LogException(System.Exception exception);
    }
}