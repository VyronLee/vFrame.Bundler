//------------------------------------------------------------
//        File:  BunderSetting.cs
//       Brief:  BunderSetting
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:18
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using vBundler.Interface;

namespace vBundler
{
    public static class BundlerCustomSettings
    {
        public static IFileReader kCustomFileReader = null;
        public static IFileReaderAsync kCustomFileReaderAsync = null;

#if UNITY_EDITOR
        public static bool kUseAssetDatabaseInsteadOfResources = true;
#endif
    }
}