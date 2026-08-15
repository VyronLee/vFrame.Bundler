// ------------------------------------------------------------
//         File: ILogHandler.cs
//        Brief: Logging abstraction: Debug/Info/Warning/Error/Exception handlers for the bundler.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 15:26
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    public interface ILogHandler
    {
        void LogDebug(string text);
        void LogInfo(string text);
        void LogWarning(string text);
        void LogError(string text);
        void LogException(System.Exception exception);
    }
}