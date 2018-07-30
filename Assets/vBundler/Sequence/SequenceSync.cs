using vBundler.Exception;
using vBundler.Loader;

namespace vBundler.Sequence
{
    public class SequenceSync : SequenceBase
    {
        public SequenceSync(LoaderBase loader) : base(loader)
        {
        }

        public override bool IsDone
        {
            get { return _loader.IsDone; }
        }

        protected override void Action()
        {
            LoadRecursive(_loader);
        }

        private void LoadRecursive(LoaderBase loader)
        {
            // Load dependencies at first
            foreach (var dependency in loader.Dependencies) LoadRecursive(dependency);

            // Load target at last
            var loaderAsync = loader as LoaderAsync;
            if (loaderAsync != null)
                if (loaderAsync.IsLoading)
                    throw new BundleMixLoadingRequestException("Mix use of asynchronously and synchronously loaders");
            loader.Load();
        }
    }
}