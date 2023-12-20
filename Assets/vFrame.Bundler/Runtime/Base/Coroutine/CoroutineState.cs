using System;

namespace vFrame.Bundler.Base.Coroutine
{
    [Flags][Serializable]
    internal enum CoroutineState
    {
        Paused = 1,
        Running = 1 << 1,
        Stopped = 1 << 2,
        Finished = 1 << 3,
    }
}