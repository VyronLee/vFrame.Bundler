// ------------------------------------------------------------
//         File: SceneAsync.cs
//        Brief: Asynchronous scene loading handle: extends Scene with IAsync polling of IsDone/Progress
//               from the underlying SceneLoader; usable as a Unity coroutine iterator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:25:07
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Asynchronous scene loading handle: extends <see cref="Scene"/> with <see cref="IAsync"/> progress
    /// polling, and matches the enumerator pattern so it can be yielded in a Unity coroutine until loading completes.
    /// </summary>
    public class SceneAsync : Scene, IAsync
    {
        /// <summary>
        /// Advances the iteration; returns <see langword="true"/> while the scene is still loading
        /// and <see langword="false"/> once loading has finished.
        /// </summary>
        /// <returns><see langword="true"/> if loading is not yet done; otherwise, <see langword="false"/>.</returns>
        public bool MoveNext()
        {
            return !IsDone;
        }

        /// <summary>Does nothing; iteration state is owned by the underlying scene loader.</summary>
        public void Reset()
        {
        }

        /// <summary>Always returns <see langword="null"/>; the handle yields no per-iteration values.</summary>
        public object Current => null;

        /// <summary>
        /// Gets a value indicating whether the scene loading has finished.
        /// </summary>
        /// <exception cref="ArgumentException">The bound loader is missing or is not a <see cref="SceneLoader"/>.</exception>
        public bool IsDone => SceneLoader.IsDone;

        /// <summary>
        /// Gets the normalized loading progress of the scene, in the range [0, 1].
        /// </summary>
        /// <exception cref="ArgumentException">The bound loader is missing or is not a <see cref="SceneLoader"/>.</exception>
        public float Progress => SceneLoader.Progress;
    }
}