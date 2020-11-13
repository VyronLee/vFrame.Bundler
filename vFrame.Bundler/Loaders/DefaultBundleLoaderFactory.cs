using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Loaders
{
    public class DefaultBundleLoaderFactory : ILoaderFactory
    {
        public BundleLoaderSync CreateLoader() {
            return new BundleLoaderSync();
        }

        public BundleLoaderAsync CreateLoaderAsync() {
            return new BundleLoaderAsync();
        }
    }
}