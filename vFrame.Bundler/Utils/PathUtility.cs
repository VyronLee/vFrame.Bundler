//------------------------------------------------------------
//        File:  PathUtility.cs
//       Brief:  PathUtility
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:15
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using vFrame.Bundler.Utils.Pools;

namespace vFrame.Bundler.Utils
{
    public static class PathUtility
    {
        public static string NormalizePath(string value)
        {
            value = value.Replace("\\", "/");
            return value;
        }

        public static string Combine(string path1, string path2)
        {
            var value = Path.Combine(path1, path2);
            return NormalizePath(value);
        }

        public static string NormalizeAssetBundlePath(string value)
        {
            if (Path.IsPathRooted(value))
                value = AbsolutePathToRelativeDataPath(value);

            value = value.ToLower();
            return NormalizePath(value);
        }

        public static string AbsolutePathToRelativeProjectPath(string fullPath)
        {
            var path = AbsolutePathToRelativeDataPath(fullPath);
            path = Path.Combine("Assets", path);
            return NormalizePath(path);
        }

        public static string AbsolutePathToRelativeDataPath(string fullPath)
        {
            fullPath = NormalizePath(fullPath);
            var projDataFullPath = NormalizePath(Path.GetFullPath(Application.dataPath) + "/");
            var relativaPath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativaPath);
        }

        public static string AbsolutePathToRelativeResourcesPath(string fullPath)
        {
            fullPath = NormalizePath(fullPath);
            var resourcesPath = Path.Combine(Application.dataPath, "Resources");
            var projDataFullPath = NormalizePath(resourcesPath + "/");
            var relativePath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativePath);
        }

        public static string AbsolutePathToRelativeStreamingAssetsPath(string fullPath)
        {
            fullPath = NormalizePath(fullPath);
            var streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");
            var projDataFullPath = NormalizePath(streamingAssetsPath + "/");
            var relativePath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativePath);
        }

        public static string RelativeDataPathToAbsolutePath(string relativePath)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            return NormalizePath(fullPath);
        }

        public static string RelativeProjectPathToAbsolutePath(string relativePath)
        {
            var dataPath = Application.dataPath;
            var projectPath = dataPath.Remove(dataPath.Length - 6, 6);
            var fullPath = Path.Combine(projectPath, relativePath);
            return NormalizePath(fullPath);
        }

        public static string RelativeResourcesPathToAbsolutePath(string relativePath)
        {
            var resourcesPath = Path.Combine(Application.dataPath, "Resources");
            var fullPath = Path.Combine(resourcesPath, relativePath);
            return NormalizePath(fullPath);
        }

        public static string RelativeProjectPathToRelativeDataPath(string relativePath)
        {
            return relativePath.Remove(0, 7);
        }

        public static string RelativeProjectPathToRelativeResourcesPath(string relativePath)
        {
            return relativePath.Remove(0, 17);
        }

        public static string RelativeDataPathToRelativeProjectPath(string relativePath)
        {
            return string.Format("Assets/{0}", relativePath);
        }

        public static string RelativeDataPathToRelativeResourcesPath(string relativePath)
        {
            return relativePath.Remove(0, 10);
        }

        public static string RelativeResourcesPathToRelativeDataPath(string relativePath)
        {
            return string.Format("Resources/{0}", relativePath);
        }

        public static string RelativeResourcesPathToRelativeProjectPath(string relativePath)
        {
            return string.Format("Assets/Resources/{0}", relativePath);
        }

        public static string GetBundleName(string value)
        {
            value = Path.Combine(
                Path.GetDirectoryName(value),
                Path.GetFileNameWithoutExtension(value));
            value = NormalizeAssetBundlePath(value);
            return value;
        }

        public static string GetAssetName(string value)
        {
            value = Path.GetFileNameWithoutExtension(value);
            return value;
        }

        public static bool IsFileInPersistentDataPath(string path)
        {
            return path.StartsWith(Application.persistentDataPath);
        }

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