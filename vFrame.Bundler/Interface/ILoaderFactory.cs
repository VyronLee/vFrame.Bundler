using vFrame.Bundler.Loaders;

namespace vFrame.Bundler.Interface
{
    public interface ILoaderFactory
    {
        BundleLoaderSync CreateLoader();
        BundleLoaderAsync CreateLoaderAsync();
    }
}