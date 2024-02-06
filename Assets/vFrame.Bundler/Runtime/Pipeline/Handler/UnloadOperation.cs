// ------------------------------------------------------------
//         File: UnloadOperation.cs
//        Brief: UnloadOperation.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-5 16:13
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public class UnloadOperation : CustomYieldInstruction
    {
        private bool _isDone;

        internal void SetDone(bool value) {
            _isDone = value;
        }

        public override bool keepWaiting => !_isDone;

        public static UnloadOperation Completed { get; } = new UnloadOperation {
            _isDone = true
        };
    }
}