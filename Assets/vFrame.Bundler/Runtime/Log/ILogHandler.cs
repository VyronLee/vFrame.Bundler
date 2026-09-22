// ------------------------------------------------------------
//         File: ILogHandler.cs
//        Brief: Receives bundler log messages at Debug, Info, Warning, Error and Exception levels.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:14:39
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Logging sink abstraction that receives diagnostic messages emitted by the bundler.
    /// </summary>
    public interface ILogHandler
    {
        /// <summary>
        /// Handles a debug-level message used for verbose diagnostics.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        void LogDebug(string text);

        /// <summary>
        /// Handles an informational message describing normal operation.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        void LogInfo(string text);

        /// <summary>
        /// Handles a warning message about a non-fatal issue.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        void LogWarning(string text);

        /// <summary>
        /// Handles an error message about a failure the bundler recovered from or reported.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        void LogError(string text);

        /// <summary>
        /// Handles an unhandled exception raised inside the bundler.
        /// </summary>
        /// <param name="exception">Exception instance to report.</param>
        void LogException(System.Exception exception);
    }
}