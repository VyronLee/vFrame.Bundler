using System.Collections.Generic;

namespace vBundler.Loader.Factory
{
    public abstract class LoaderFactory
    {
        public abstract LoaderBase CreateLoader(string path, List<string> searchPath);
    }
}