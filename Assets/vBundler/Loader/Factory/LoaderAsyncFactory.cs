using System.Collections.Generic;

namespace vBundler.Loader.Factory
{
    public class LoaderAsyncFactory : LoaderFactory
    {
        public override LoaderBase CreateLoader(string path, List<string> searchPath)
        {
            return new LoaderAsync(path, searchPath);
        }
    }
}