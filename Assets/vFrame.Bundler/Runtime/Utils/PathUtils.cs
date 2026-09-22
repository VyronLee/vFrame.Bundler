// ------------------------------------------------------------
//         File: PathUtils.cs
//        Brief: Unity path helpers: normalization, combination, relative-path conversions, bundle names, MD5 hashing.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:11:56
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Static path helpers for Unity projects: normalization, combination, conversion between absolute and
    ///     project/data/Resources/StreamingAssets-relative forms, asset bundle naming and MD5 path hashing.
    /// </summary>
    public static class PathUtils
    {
        /// <summary>Replaces every backslash in the path with a forward slash.</summary>
        /// <param name="value">Path to normalize.</param>
        /// <returns>The path using '/' separators.</returns>
        public static string NormalizePath(string value)
        {
            value = value.Replace("\\", "/");
            return value;
        }

        /// <summary>Combines two paths and normalizes separators.</summary>
        /// <param name="path1">First path segment.</param>
        /// <param name="path2">Second path segment.</param>
        /// <returns>The combined, normalized path.</returns>
        public static string Combine(string path1, string path2)
        {
            var value = Path.Combine(path1, path2);
            return NormalizePath(value);
        }

        /// <summary>Combines a sequence of path segments and normalizes separators.</summary>
        /// <param name="paths">Path segments in order; the first may be absolute.</param>
        /// <returns>The combined, normalized path, or null when <paramref name="paths"/> is null.</returns>
        public static string Combine(params string[] paths)
        {
            if (null == paths) return null;
            if (paths.Length <= 1) return NormalizePath(paths[0]);
            var ret = paths[0];
            for (var i = 1; i < paths.Length; i++) ret = Path.Combine(ret, paths[i]);
            return NormalizePath(ret);
        }

        /// <summary>
        ///     Converts a path into its canonical bundle name form: project-relative, lowercased, forward slashes.
        /// </summary>
        /// <param name="value">Absolute or project-relative asset path.</param>
        /// <returns>The lowercased project-relative path used as the bundle name.</returns>
        public static string NormalizeAssetBundlePath(string value)
        {
            if (Path.IsPathRooted(value))
                value = AbsolutePathToRelativeProjectPath(value);

            value = value.ToLower();
            return NormalizePath(value);
        }

        /// <summary>Converts an absolute path inside the project to an "Assets/..." relative path.</summary>
        /// <param name="fullPath">Absolute path under the project data folder.</param>
        /// <returns>The project-relative path starting with "Assets/".</returns>
        public static string AbsolutePathToRelativeProjectPath(string fullPath)
        {
            var path = AbsolutePathToRelativeDataPath(fullPath);
            path = Path.Combine("Assets", path);
            return NormalizePath(path);
        }

        /// <summary>Strips the project data path prefix, yielding a path relative to the data folder.</summary>
        /// <param name="fullPath">Absolute path under the project data folder.</param>
        /// <returns>The data-relative path, without an "Assets/" prefix.</returns>
        public static string AbsolutePathToRelativeDataPath(string fullPath)
        {
            fullPath = NormalizePath(fullPath);
            var projDataFullPath = NormalizePath(Path.GetFullPath(Application.dataPath) + "/");
            var relativePath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativePath);
        }

        /// <summary>Strips the "Assets/Resources/" prefix, yielding a path relative to the root Resources folder.</summary>
        /// <param name="fullPath">Absolute path inside the root Resources folder.</param>
        /// <returns>The Resources-relative path.</returns>
        public static string AbsolutePathToRelativeResourcesPath(string fullPath)
        {
            fullPath = NormalizePath(fullPath);
            var resourcesPath = Path.Combine(Application.dataPath, "Resources");
            var projDataFullPath = NormalizePath(resourcesPath + "/");
            var relativePath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativePath);
        }

        /// <summary>Strips the StreamingAssets folder prefix, yielding a path relative to it.</summary>
        /// <param name="fullPath">Absolute path inside the StreamingAssets folder.</param>
        /// <returns>The StreamingAssets-relative path.</returns>
        public static string AbsolutePathToRelativeStreamingAssetsPath(string fullPath)
        {
            fullPath = NormalizePath(fullPath);
            var projDataFullPath = NormalizePath(Application.streamingAssetsPath + "/");
            var relativePath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativePath);
        }

        /// <summary>Resolves a data-relative path against the project data folder.</summary>
        /// <param name="relativePath">Path relative to the data folder, without an "Assets/" prefix.</param>
        /// <returns>The absolute path.</returns>
        public static string RelativeDataPathToAbsolutePath(string relativePath)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            return NormalizePath(fullPath);
        }

        /// <summary>Resolves an "Assets/..." relative path against the project root.</summary>
        /// <param name="relativePath">Project-relative path starting with "Assets/".</param>
        /// <returns>The absolute path.</returns>
        public static string RelativeProjectPathToAbsolutePath(string relativePath)
        {
            var dataPath = Application.dataPath;
            var projectPath = dataPath.Remove(dataPath.Length - 6, 6);
            var fullPath = Path.Combine(projectPath, relativePath);
            return NormalizePath(fullPath);
        }

        /// <summary>Resolves a Resources-relative path against the root Resources folder.</summary>
        /// <param name="relativePath">Path relative to the Resources folder.</param>
        /// <returns>The absolute path.</returns>
        public static string RelativeResourcesPathToAbsolutePath(string relativePath)
        {
            var resourcesPath = Path.Combine(Application.dataPath, "Resources");
            var fullPath = Path.Combine(resourcesPath, relativePath);
            return NormalizePath(fullPath);
        }

        /// <summary>Removes the leading "Assets/" segment from a project-relative path.</summary>
        /// <param name="relativePath">Project-relative path starting with "Assets/".</param>
        /// <returns>The data-relative path.</returns>
        public static string RelativeProjectPathToRelativeDataPath(string relativePath)
        {
            return relativePath.Remove(0, 7);
        }

        /// <summary>Removes the leading "Assets/Resources/" segment from a project-relative path.</summary>
        /// <param name="relativePath">Project-relative path starting with "Assets/Resources/".</param>
        /// <returns>The Resources-relative path.</returns>
        public static string RelativeProjectPathToRelativeResourcesPath(string relativePath)
        {
            return relativePath.Remove(0, 17);
        }

        /// <summary>Prefixes a data-relative path with "Assets/".</summary>
        /// <param name="relativePath">Data-relative path.</param>
        /// <returns>The project-relative path starting with "Assets/".</returns>
        public static string RelativeDataPathToRelativeProjectPath(string relativePath)
        {
            return string.Format("Assets/{0}", relativePath);
        }

        /// <summary>Removes the leading "Resources/" segment from a data-relative path.</summary>
        /// <param name="relativePath">Data-relative path starting with "Resources/".</param>
        /// <returns>The Resources-relative path.</returns>
        public static string RelativeDataPathToRelativeResourcesPath(string relativePath)
        {
            return relativePath.Remove(0, 10);
        }

        /// <summary>Prefixes a Resources-relative path with "Resources/".</summary>
        /// <param name="relativePath">Resources-relative path.</param>
        /// <returns>The data-relative path starting with "Resources/".</returns>
        public static string RelativeResourcesPathToRelativeDataPath(string relativePath)
        {
            return string.Format("Resources/{0}", relativePath);
        }

        /// <summary>Prefixes a Resources-relative path with "Assets/Resources/".</summary>
        /// <param name="relativePath">Resources-relative path.</param>
        /// <returns>The project-relative path starting with "Assets/Resources/".</returns>
        public static string RelativeResourcesPathToRelativeProjectPath(string relativePath)
        {
            return string.Format("Assets/Resources/{0}", relativePath);
        }

        /// <summary>Returns the project root folder derived from the Unity data path.</summary>
        /// <returns>Absolute path to the folder containing the "Assets" folder.</returns>
        public static string ProjectPath()
        {
            return Application.dataPath.Remove(Application.dataPath.Length - 7, 7);
        }

        /// <summary>
        ///     Builds the bundle name for an asset path: directory plus file name without extension,
        ///     normalized to lowercase project-relative form.
        /// </summary>
        /// <param name="value">Asset path.</param>
        /// <returns>The canonical bundle name.</returns>
        public static string GetBundleName(string value)
        {
            value = Path.Combine(
                Path.GetDirectoryName(value),
                Path.GetFileNameWithoutExtension(value));
            value = NormalizeAssetBundlePath(value);
            return value;
        }

        /// <summary>Returns the file name of a path without its extension.</summary>
        /// <param name="value">Asset path.</param>
        /// <returns>The file name without extension.</returns>
        public static string GetAssetName(string value)
        {
            value = Path.GetFileNameWithoutExtension(value);
            return value;
        }

        /// <summary>Checks whether a path starts with the platform persistent data folder.</summary>
        /// <param name="path">Path to test.</param>
        /// <returns>True when the path is inside the persistent data folder.</returns>
        public static bool IsFileInPersistentDataPath(string path)
        {
            return path.StartsWith(Application.persistentDataPath);
        }

        /// <summary>
        ///     Hashes a path with MD5 and formats the lowercase hex digest as "xx/yy/fullhex", so the first
        ///     two two-character segments act as shard directories when used as a file path.
        /// </summary>
        /// <param name="path">Path string to hash.</param>
        /// <returns>The hashed path string, e.g. "ab/cd/abcdef...".</returns>
        public static string HashPath(string path)
        {
            var md5 = new MD5CryptoServiceProvider();
            var ret = md5.ComputeHash(Encoding.UTF8.GetBytes(path));

            var builder = StringBuilderPool.Get();
            foreach (var b in ret)
                builder.Append(b.ToString("x2"));
            var str = builder.ToString();
            StringBuilderPool.Return(builder);

            builder = StringBuilderPool.Get();
            builder.Append(str.Substring(0, 2));
            builder.Append("/");
            builder.Append(str.Substring(2, 2));
            builder.Append("/");
            builder.Append(str);
            var hashed = builder.ToString();
            StringBuilderPool.Return(builder);

            return hashed;
        }
    }
}