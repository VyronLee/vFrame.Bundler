using vBundler.Loader;

namespace vBundler.Sequence
{
    public abstract class SequenceBase
    {
        protected LoaderBase _loader;

        protected SequenceBase(LoaderBase loader)
        {
            _loader = loader;

            Action();
        }

        public abstract bool IsDone { get; }

        protected abstract void Action();
    }
}