using System.Collections.Generic;
using vBundler.Interface;
using vBundler.Loader;

namespace vBundler.Sequence
{
    public class SequenceAsync : SequenceBase, IAsync
    {
        private readonly Stack<LoaderBase> _loaders = new Stack<LoaderBase>();
        private LoaderBase _current;

        public SequenceAsync(LoaderBase loader) : base(loader)
        {
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        public override bool IsDone
        {
            get
            {
                if (_current != null)
                {
                    if (_current.IsDone)
                        _current = null;
                    return false;
                }

                if (_loaders.Count <= 0)
                    return true;

                _current = _loaders.Pop();
                _current.Load();

                return false;
            }
        }

        protected override void Action()
        {
            TravelRecursive(_loader);
        }

        private void TravelRecursive(LoaderBase loader)
        {
            _loaders.Push(loader);

            foreach (var dependency in loader.Dependencies) TravelRecursive(dependency);
        }
    }
}