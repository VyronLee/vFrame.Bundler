using vFrame.Bundler.Base.Coroutine;

namespace vFrame.Bundler
{
    internal class BundlerContext
    {
        public BundlerOptions Options { get; set; }
        public CoroutinePool CoroutinePool { get; set; }
    }
}