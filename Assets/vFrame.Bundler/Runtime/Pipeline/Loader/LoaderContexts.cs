// ------------------------------------------------------------
//         File: LoaderContexts.cs
//        Brief: Parameter struct passed to loader constructors: asset path/type/load mode, scene mode, parent loader.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 20:8
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    internal struct LoaderContexts
    {
        public string AssetPath;
        public AssetLoadType AssetLoadType;
        public Type AssetType;
        public LoadSceneMode SceneMode;
        public Loader ParentLoader;
    }
}