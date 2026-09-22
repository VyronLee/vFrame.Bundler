// ------------------------------------------------------------
//         File: AssetHelper.cs
//        Brief: Editor AssetDatabase helpers: path/asset-kind predicates and buildable asset/dependency enumeration.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:07:42
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Editor-only helpers over <see cref="AssetDatabase" />: path and asset-kind predicates plus
    ///     buildable-asset and dependency enumeration used by the bundle build pipeline.
    /// </summary>
    internal static class AssetHelper
    {
        /// <summary>Converts an asset GUID to its project-relative asset path.</summary>
        /// <param name="guid">Asset GUID to resolve.</param>
        /// <returns>The asset path, or an empty string if the GUID is unknown.</returns>
        public static string GuidToPath(string guid)
        {
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        /// <summary>Determines whether the given project-relative path is a valid folder.</summary>
        /// <param name="path">Project-relative path to test.</param>
        public static bool IsFolder(string path)
        {
            return AssetDatabase.IsValidFolder(path);
        }

        /// <summary>Determines whether the given path is treated as a file (anything that is not a valid folder).</summary>
        /// <param name="path">Project-relative path to test.</param>
        public static bool IsFile(string path)
        {
            return !IsFolder(path);
        }

        /// <summary>
        ///     Determines whether the main asset at the given path belongs to the <c>UnityEditor</c> namespace
        ///     (an editor-only asset). Returns <c>false</c> when no asset exists at the path.
        /// </summary>
        /// <param name="path">Project-relative asset path to test.</param>
        public static bool IsEditorAsset(string path)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (null == type) {
                return false;
            }
            return type.Namespace == nameof(UnityEditor);
        }

        /// <summary>Determines whether the asset at the given path does not belong to the <c>UnityEditor</c> namespace.</summary>
        /// <param name="path">Project-relative asset path to test.</param>
        public static bool IsNotEditorAsset(string path)
        {
            return !IsEditorAsset(path);
        }

        /// <summary>Determines whether the given file name or path has a <c>.shader</c> extension.</summary>
        /// <param name="name">File name or path to test.</param>
        public static bool IsShader(string name)
        {
            return Path.GetExtension(name) == ".shader";
        }

        /// <summary>Determines whether the given file name or path has a <c>.unity</c> extension (a scene).</summary>
        /// <param name="name">File name or path to test.</param>
        public static bool IsScene(string name)
        {
            return Path.GetExtension(name) == ".unity";
        }

        /// <summary>Determines whether the given file name or path has a <c>.asset</c> extension (a ScriptableObject).</summary>
        /// <param name="name">File name or path to test.</param>
        public static bool IsScriptableObject(string name)
        {
            return Path.GetExtension(name) == ".asset";
        }

        /// <summary>Determines whether the given file name or path has a <c>.cs</c> extension (a C# script).</summary>
        /// <param name="name">File name or path to test.</param>
        public static bool IsScript(string name)
        {
            return Path.GetExtension(name) == ".cs";
        }

        /// <summary>Determines whether the given file name or path has a <c>.dll</c> extension (a compiled assembly).</summary>
        /// <param name="name">File name or path to test.</param>
        public static bool IsAssembly(string name)
        {
            return Path.GetExtension(name) == ".dll";
        }

        /// <summary>Determines whether the given file name or path has a <c>.meta</c> extension (a Unity meta file).</summary>
        /// <param name="name">File name or path to test.</param>
        public static bool IsMeta(string name)
        {
            return Path.GetExtension(name) == ".meta";
        }

        /// <summary>
        ///     Determines whether the given name refers to one of Unity's built-in resource files
        ///     (<c>unity_builtin_extra</c> or <c>unity default resources</c>).
        /// </summary>
        /// <param name="name">Resource name to test.</param>
        public static bool IsBuiltinResource(string name)
        {
            return name.EndsWith("unity_builtin_extra")
                || name.EndsWith("unity default resources");
        }

        /// <summary>Determines whether the given path resides inside the project's <c>Assets/</c> or <c>Packages/</c> roots.</summary>
        /// <param name="name">Path to test.</param>
        public static bool IsProjectAssets(string name)
        {
            return name.StartsWith("Assets/")
                || name.StartsWith("Packages/");
        }

        /// <summary>
        ///     Determines whether the asset at the given path can be included in an AssetBundle build:
        ///     scenes and any non-editor asset qualify.
        /// </summary>
        /// <param name="name">Asset path to test.</param>
        public static bool IsBuildableAssets(string name)
        {
            return IsScene(name) || IsNotEditorAsset(name); // 'UnityEditor.SceneAsset' can also be built into AssetBundle
        }

        /// <summary>
        ///     Enumerates all buildable assets under the given project path. Logs a warning and returns an
        ///     empty sequence when the path is outside the <c>Assets/</c> or <c>Packages/</c> roots.
        /// </summary>
        /// <param name="path">Project-relative search path; must start with <c>Assets/</c> or <c>Packages/</c>.</param>
        /// <returns>Distinct buildable asset paths under the path, or an empty sequence on an invalid path.</returns>
        public static IEnumerable<string> FindAllAssets(string path)
        {
            if (!IsProjectAssets(path)) {
                Debug.LogWarning($"Argument is not project resource path: {path}, "
                                 + "only path start with 'Assets/' or 'Packages/' is allowed!");
                return Array.Empty<string>();
            }

            return AssetDatabase.FindAssets("", new[] { path.TrimEnd('/') })
                .Select(GuidToPath)
                .Where(IsFile)
                .Where(IsBuildableAssets)
                .Distinct();
        }

        /// <summary>
        ///     Enumerates the distinct set of buildable project assets that the given asset paths depend on
        ///     (transitively, inputs included), filtered to the <c>Assets/</c> and <c>Packages/</c> roots.
        /// </summary>
        /// <param name="paths">Asset paths whose dependency closure is collected.</param>
        /// <returns>Distinct buildable project asset paths from the dependency closure.</returns>
        public static IEnumerable<string> GetAllDependencies(string[] paths)
        {
            return AssetDatabase.GetDependencies(paths, true)
                .Where(IsProjectAssets)
                .Where(IsBuildableAssets)
                .Distinct();
        }
    }
}