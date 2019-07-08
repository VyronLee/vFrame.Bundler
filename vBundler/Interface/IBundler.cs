//------------------------------------------------------------
//        File:  IBundler.cs
//       Brief:  Bundler interface.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:05
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

namespace vBundler.Interface
{
    public enum BundleModeType
    {
        Resource, // Load from resources.
        Bundle // Load from bundles.
    }

    public interface IBundler
    {
        ILoadRequest LoadAsset(string path);
        ILoadRequestAsync LoadAssetAsync(string path);

        void AddSearchPath(string path);
        void ClearSearchPaths();

        void GarbageCollect();

        void SetLogLevel(int level);
        void SetMode(BundleModeType type);
    }
}