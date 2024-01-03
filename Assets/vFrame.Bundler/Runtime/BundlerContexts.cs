using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class BundlerContexts
    {
        public Bundler Bundler { get; set; }
        public BundlerOptions Options { get; set; }
        public BundlerManifest Manifest { get; set; }

        public List<Loader> Loaders { get; } = new List<Loader>();
        public List<LoaderHandler> LoaderHandlers { get; } = new List<LoaderHandler>();
        public Dictionary<string, Asset> Assets { get; } = new Dictionary<string, Asset>();
    }
}