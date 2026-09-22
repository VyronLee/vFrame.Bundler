// ------------------------------------------------------------
//         File: LoaderContexts.cs
//        Brief: Immutable parameter bundle passed to loader constructors describing the asset request.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:50:24
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using UnityEngine.SceneManagement;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Describes a pending load request: what to load (path, type, load mode), how scenes open,
    ///    and which parent loader owns the resulting loader.
    /// </summary>
    internal struct LoaderContexts
    {
        /// <summary>Project-relative path of the asset or scene to load.</summary>
        public string AssetPath;

        /// <summary>How the asset was requested; selects the concrete loader implementation.</summary>
        public AssetLoadType AssetLoadType;

        public Type AssetType;

        /// <summary>Unity load mode applied when the requested asset is a scene.</summary>
        public LoadSceneMode SceneMode;

        /// <summary>
        ///     Parent loader in the dependency chain; retained and released together with the child.
        /// </summary>
        public Loader ParentLoader;
    }
}