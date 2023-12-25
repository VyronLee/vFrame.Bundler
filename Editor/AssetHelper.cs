// ------------------------------------------------------------
//         File: AssetHelper.cs
//        Brief: AssetHelper.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 23:55
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.IO;
using UnityEditor;

namespace vFrame.Bundler.Editor
{
    internal static class AssetHelper
    {
        public static string GuidToPath(string guid) {
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        public static bool IsFolder(string path) {
            return AssetDatabase.IsValidFolder(path);
        }

        public static bool IsShader(string name) {
            return Path.GetExtension(name) == ".shader";
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

        public static bool IsProjectResource(string name) {
            return name.StartsWith("Assets/")
                || name.StartsWith("Packages/");
        }
    }
}