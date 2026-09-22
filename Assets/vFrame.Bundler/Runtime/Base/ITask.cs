// ------------------------------------------------------------
//         File: ITask.cs
//        Brief: Step-driven task contract with a lifecycle state and Start/Stop/Update control methods.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:33:45
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Lifecycle state of an <see cref="ITask"/>.
    /// </summary>
    public enum TaskState
    {
        /// <summary>Created but never started, or stopped before completion.</summary>
        NotStarted,

        /// <summary>Started and awaiting per-frame <see cref="ITask.Update"/> calls until done.</summary>
        Processing,

        /// <summary>Completed successfully; terminal until stopped or restarted.</summary>
        Finished,

        /// <summary>Terminated due to a failure; terminal until stopped or restarted.</summary>
        Error,
    }

    /// <summary>
    ///     A step-driven task whose asynchronous work advances one step per <see cref="Update"/> call.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        ///     Gets the current lifecycle state of the task.
        /// </summary>
        TaskState TaskState { get; }

        /// <summary>
        ///     Starts the task, transitioning it into <see cref="TaskState.Processing"/>;
        ///     implementations typically ignore calls from any other state.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops the task and releases its resources, resetting it to <see cref="TaskState.NotStarted"/>;
        ///     implementations typically ignore calls unless the task is processing or finished.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Advances the task by one step, eventually moving it to <see cref="TaskState.Finished"/> or <see cref="TaskState.Error"/>;
        ///     implementations typically ignore calls unless the task is processing.
        /// </summary>
        void Update();
    }
}