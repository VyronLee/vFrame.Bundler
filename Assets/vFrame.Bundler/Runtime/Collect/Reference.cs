// ------------------------------------------------------------
//         File: Reference.cs
//        Brief: Simple reference counter tracking Retain/Release balance and throwing on release underflow.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:00:02
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Simple reference counter implementing <see cref="IReference" />; subclass and override the members to add
    ///     custom retain/release side effects.
    /// </summary>
    public class Reference : IReference
    {
        /// <summary>Current reference count; never negative.</summary>
        private int _references;

        /// <summary>Increments the reference count by one.</summary>
        public virtual void Retain()
        {
            ++_references;
        }

        /// <summary>
        ///     Decrements the reference count by one.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when releasing more times than retaining.</exception>
        public virtual void Release()
        {
            if (_references <= 0) {
                throw new System.InvalidOperationException(
                    "Release() called more times than Retain(); reference count is already at zero.");
            }
            --_references;
        }

        /// <summary>Gets the current reference count.</summary>
        public virtual int References => _references;
    }
}