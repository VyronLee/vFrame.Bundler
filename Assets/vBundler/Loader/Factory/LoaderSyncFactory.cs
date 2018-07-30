using System.Collections.Generic;

namespace vBundler.Loader.Factory
{
    public class LoaderSyncFactory : LoaderFactory
    {
        public override LoaderBase CreateLoader(string path, List<string> searchPath)
        {
            return new LoaderSync(path, searchPath);
        }
    }
}