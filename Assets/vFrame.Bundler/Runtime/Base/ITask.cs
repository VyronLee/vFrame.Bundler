// ------------------------------------------------------------
//         File: ITask.cs
//        Brief: ITask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-2 22:51
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    public enum TaskState
    {
        NotStarted,
        Processing,
        Finished,
        Error,
    }

    public interface ITask
    {
        TaskState TaskState { get; }
        void Start();
        void Stop();
        void Update();
    }
}