// ------------------------------------------------------------
//         File: IAssetBundleCreateAdapter.cs
//        Brief: Abstraction over AssetBundle instantiation (sync and async), letting VFS or others supply bundle files.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 16:1
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    public interface IAssetBundleCreateAdapter
    {
        AssetBundle CreateAssetBundle(string bundlePath);
        AssetBundleCreateRequest CreateRequest(string bundlePath);
    }
}