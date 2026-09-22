// ------------------------------------------------------------
//         File: LogSystem.cs
//        Brief: Logging facade with severity filtering that forwards messages to a swappable ILogHandler backend.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:14:47
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Bundler log system that filters messages by severity level and forwards
    /// the accepted ones to the currently installed <see cref="ILogHandler"/> backend.
    /// </summary>
    internal class LogSystem : BundlerSystem, ILogger
    {
        /// <summary>
        /// Minimum severity level a message must reach to be emitted; lower-severity messages are dropped.
        /// </summary>
        private int _level;

        /// <summary>
        /// Backend sink that actually outputs the messages; never used when null.
        /// </summary>
        private ILogHandler _logHandler;

        /// <summary>
        /// Initializes the log system with the default internal handler.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler contexts.</param>
        public LogSystem(BundlerContexts bundlerContexts) : base(bundlerContexts)
        {
            _logHandler = new InternalLogHandler();
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {

        }

        /// <inheritdoc />
        protected override void OnUpdate()
        {

        }

        /// <summary>
        /// Sets the minimum severity level to emit; messages below this level are dropped.
        /// </summary>
        /// <param name="level">Minimum severity level, one of the <see cref="LogLevel"/> constants.</param>
        public void SetLogLevel(int level)
        {
            _level = level;
        }

        /// <summary>
        /// Logs a debug-level diagnostic message.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        public void LogDebug(string text)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(text);
        }

        /// <summary>
        /// Logs a debug-level diagnostic message formatted with one argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogDebug<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(string.Format(text, arg1));
        }

        /// <summary>
        /// Logs a debug-level diagnostic message formatted with two arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogDebug<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(string.Format(text, arg1, arg2));
        }

        /// <summary>
        /// Logs a debug-level diagnostic message formatted with three arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <typeparam name="T3">Type of the third format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogDebug<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Debug) {
                return;
            }
            _logHandler?.LogDebug(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        /// Logs an informational message about normal operation.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        public void LogInfo(string text)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(text);
        }

        /// <summary>
        /// Logs an informational message formatted with one argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogInfo<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(string.Format(text, arg1));
        }

        /// <summary>
        /// Logs an informational message formatted with two arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogInfo<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(string.Format(text, arg1, arg2));
        }

        /// <summary>
        /// Logs an informational message formatted with three arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <typeparam name="T3">Type of the third format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogInfo<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Info) {
                return;
            }
            _logHandler?.LogInfo(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        /// Logs a warning message about a non-fatal issue.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        public void LogWarning(string text)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(text);
        }

        /// <summary>
        /// Logs a warning message formatted with one argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogWarning<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(string.Format(text, arg1));
        }

        /// <summary>
        /// Logs a warning message formatted with two arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogWarning<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(string.Format(text, arg1, arg2));
        }

        /// <summary>
        /// Logs a warning message formatted with three arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <typeparam name="T3">Type of the third format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogWarning<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Warning) {
                return;
            }
            _logHandler?.LogWarning(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        /// Logs an error message about a failure the bundler recovered from or reported.
        /// </summary>
        /// <param name="text">Message text to output.</param>
        public void LogError(string text)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(text);
        }

        /// <summary>
        /// Logs an error message formatted with one argument.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        public void LogError<T1>(string text, T1 arg1)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(string.Format(text, arg1));
        }

        /// <summary>
        /// Logs an error message formatted with two arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        public void LogError<T1, T2>(string text, T1 arg1, T2 arg2)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(string.Format(text, arg1, arg2));
        }

        /// <summary>
        /// Logs an error message formatted with three arguments.
        /// </summary>
        /// <typeparam name="T1">Type of the first format argument.</typeparam>
        /// <typeparam name="T2">Type of the second format argument.</typeparam>
        /// <typeparam name="T3">Type of the third format argument.</typeparam>
        /// <param name="text">Composite format string of the message.</param>
        /// <param name="arg1">First format argument.</param>
        /// <param name="arg2">Second format argument.</param>
        /// <param name="arg3">Third format argument.</param>
        public void LogError<T1, T2, T3>(string text, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_level > LogLevel.Error) {
                return;
            }
            _logHandler?.LogError(string.Format(text, arg1, arg2, arg3));
        }

        /// <summary>
        /// Logs an unhandled exception raised inside the bundler.
        /// </summary>
        /// <param name="exception">Exception instance to report.</param>
        public void LogException(System.Exception exception)
        {
            if (_level > LogLevel.Exception) {
                return;
            }
            _logHandler?.LogException(exception);
        }

        /// <summary>
        /// Replaces the backend sink that receives emitted messages.
        /// </summary>
        /// <param name="handler">New sink to install.</param>
        public void SetLogHandler(ILogHandler handler)
        {
            _logHandler = handler;
        }
    }
}