// ------------------------------------------------------------
//         File: AssetHelper.cs
//        Brief: AssetHelper.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 23:55
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vFrame.Bundler.Helper
{
    internal static class AssetHelper
    {
        public static string GuidToPath(string guid) {
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        public static bool IsFolder(string path) {
            return AssetDatabase.IsValidFolder(path);
        }

        public static bool IsFile(string path) {
            return !IsFolder(path);
        }

        public static bool IsEditorAsset(string path) {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (null == type) {
                return false;
            }
            return type.Namespace == nameof(UnityEditor);
        }

        public static bool IsNotEditorAsset(string path) {
            return !IsEditorAsset(path);
        }

        public static bool IsShader(string name) {
            return Path.GetExtension(name) == ".shader";
        }

        public static bool IsScene(string name) {
            return Path.GetExtension(name) == ".unity";
        }

        public static bool IsScriptableObject(string name) {
            return Path.GetExtension(name) == ".asset";
        }

        public static bool IsScript(string name) {
            return Path.GetExtension(name) == ".cs";
        }

        public static bool IsAssembly(string name) {
            return Path.GetExtension(name) == ".dll";
        }

        public static bool IsMeta(string name) {
            return Path.GetExtension(name) == ".meta";
        }

        public static bool IsBuiltinResource(string name) {
            return name.EndsWith("unity_builtin_extra")
                || name.EndsWith("unity default resources");
        }

        public static bool IsProjectAssets(string name) {
            return name.StartsWith("Assets/")
                || name.StartsWith("Packages/");
        }

        public static bool IsBuildableAssets(string name) {
            return IsScene(name) || IsNotEditorAsset(name); // 'UnityEditor.SceneAsset' can also be built into AssetBundle
        }

        public static IEnumerable<string> FindAllAssets(string path) {
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

        public static IEnumerable<string> GetAllDependencies(string[] paths) {
            return AssetDatabase.GetDependencies(paths, true)
                .Where(IsProjectAssets)
                .Where(IsBuildableAssets)
                .Distinct();
        }
    }
}