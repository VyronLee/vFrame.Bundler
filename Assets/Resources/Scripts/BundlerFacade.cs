using System.Collections;
using System.IO;
using UnityEngine;
using vBundler;
using vBundler.Interface;
using vBundler.Utils;

namespace Sample.Scripts
{
    public class BundlerFacade : MonoBehaviour
    {
        private static BundlerFacade _instance;
        private IBundler _vBundler;

        public static BundlerFacade Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                return _instance = new GameObject("Bundler").AddComponent<BundlerFacade>();
            }
        }

        private void Awake()
        {
            var manifestFilePath =
                Path.Combine(BundlerSetting.kDefaultBundlePath, BundlerSetting.kDefaultManifestFileName);
            var manifestFullPath = PathUtility.RelativeDataPathToFullPath(manifestFilePath);
            var manifestText = File.ReadAllText(manifestFullPath);
            var manifest = JsonUtility.FromJson<BundlerManifest>(manifestText);
            var searchPath = PathUtility.RelativeDataPathToFullPath(BundlerSetting.kDefaultBundlePath);

            _vBundler = new Bundler(manifest);
            _vBundler.AddSearchPath(searchPath);
        }

        public IAsset Load<T>(string path) where T : Object
        {
            return _vBundler.Load<T>(path);
        }

        public IEnumerator LoadAsync<T>(string path, IBundler.AsyncLoadCallback callback) where T : Object
        {
            return _vBundler.LoadAsync<T>(path, callback);
        }

        public void SetLogLevel(int level)
        {
            _vBundler.SetLogLevel(level);
        }

        private void Update()
        {
            _vBundler.GarbageCollect();
        }
    }
}