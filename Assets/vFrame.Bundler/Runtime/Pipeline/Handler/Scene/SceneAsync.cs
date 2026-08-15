// ------------------------------------------------------------
//         File: SceneAsync.cs
//        Brief: Async scene handle extending Scene with IAsync: exposes IsDone/Progress
//               polled from the underlying SceneLoader.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-4 14:26
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    public class SceneAsync : Scene, IAsync
    {
        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        public object Current => null;
        public bool IsDone => SceneLoader.IsDone;
        public float Progress => SceneLoader.Progress;
    }
}