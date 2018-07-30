using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace vBundler.Interface
{
    public abstract class IBundler
    {
        public delegate void AsyncLoadCallback(IAssetAsync asset);

        public abstract IAsset Load<T>(string path) where T : Object;
        public abstract IAsset Load(string path, Type type);

        public abstract IEnumerator LoadAsync<T>(string path, AsyncLoadCallback callback) where T : Object;
        public abstract IEnumerator LoadAsync(string path, Type type, AsyncLoadCallback callback);

        public abstract void AddSearchPath(string path);
        public abstract void ClearSearchPaths();

        public abstract void GarbageCollect();

        public abstract void SetLogLevel(int level);
    }
}