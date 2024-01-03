// ------------------------------------------------------------
//         File: IAssetBundleCreateRequestAdapter.cs
//        Brief: IAssetBundleCreateRequestAdapter.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 16:1
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    public interface IAssetBundleCreateRequestAdapter
    {
        AssetBundle CreateRequest(string bundlePath);
        AssetBundleCreateRequest CreateRequestAsync(string bundlePath);
    }
}