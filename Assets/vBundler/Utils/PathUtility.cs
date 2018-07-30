using System.IO;
using UnityEngine;

namespace vBundler.Utils
{
    public static class PathUtility
    {
        public static string NormalizePath(string value)
        {
            value = value.Replace("\\", "/");
            return value;
        }

        public static string NormalizeAssetBundlePath(string value)
        {
            if (Path.IsPathRooted(value))
                value = FullPathToRelativeDataPath(value);

            value = value.ToLower();
            return value;
        }

        public static string FullPathToRelativeProjectPath(string fullPath)
        {
            var path = FullPathToRelativeDataPath(fullPath);
            path = Path.Combine("Assets", path);
            return NormalizePath(path);
        }

        public static string FullPathToRelativeDataPath(string fullPath)
        {
            var projDataFullPath = NormalizePath(Path.GetFullPath(Application.dataPath) + "/");
            var relativaPath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativaPath);
        }

        public static string FullPathToRelativeResourcesPath(string fullPath)
        {
            var resourcesPath = Path.Combine(Application.dataPath, "Resources");
            var projDataFullPath = NormalizePath(Path.GetFullPath(resourcesPath) + "/");
            var relativaPath = fullPath.Replace(projDataFullPath, "");
            return NormalizePath(relativaPath);
        }

        public static string RelativeDataPathToFullPath(string relativePath)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            return NormalizePath(fullPath);
        }

        public static string RelativeProjectPathToFullPath(string relativePath)
        {
            var dataPath = Application.dataPath;
            var projectPath = dataPath.Remove(-6, dataPath.Length);
            var fullPath = Path.Combine(projectPath, relativePath);
            return NormalizePath(fullPath);
        }

        public static string RelativeResourcesPathToFullPath(string relativePath)
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
    }
}