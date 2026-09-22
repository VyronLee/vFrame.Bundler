// ------------------------------------------------------------
//         File: UnloadOperation.cs
//        Brief: Yield instruction awaiting unload completion, with a pre-completed
//               shared instance.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:25:11
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Yield instruction that stays pending until the associated unload completes.
    /// </summary>
    public class UnloadOperation : CustomYieldInstruction
    {
        /// <summary>Completion flag of the underlying unload.</summary>
        private bool _isDone;

        /// <summary>
        /// Marks the operation as completed or still pending.
        /// </summary>
        /// <param name="value">True if the unload has completed.</param>
        internal void SetDone(bool value)
        {
            _isDone = value;
        }

        /// <summary>
        /// True while the unload is still in progress; false once it completes.
        /// </summary>
        public override bool keepWaiting => !_isDone;

        /// <summary>
        /// Shared already-completed instance that yields immediately when awaited.
        /// </summary>
        public static UnloadOperation Completed { get; } = new UnloadOperation {
            _isDone = true
        };
    }
}