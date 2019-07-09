//------------------------------------------------------------
//        File:  ResourceMode.cs
//       Brief:  ResourceMode
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:13
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using vBundler.Asset.Resource;
using vBundler.Interface;
using vBundler.LoadRequests;

namespace vBundler.Mode
{
    public class ResourceMode : ModeBase
    {
        public ResourceMode(BundlerManifest manifest, List<string> searchPaths) : base(manifest, searchPaths)
        {
        }

        public override ILoadRequest Load(string path)
        {
            return new LoadRequest(this, path, null);
        }

        public override ILoadRequestAsync LoadAsync(string path)
        {
            return new LoadRequestAsync(this, path, null);
        }

        public override IAsset GetAsset(LoadRequest request, Type type)
        {
            return new ResourceAssetSync(request.AssetPath, type);
        }

        public override IAssetAsync GetAssetAsync(LoadRequest request, Type type)
        {
            return new ResourceAssetAsync(request.AssetPath, type);
        }
    }
}