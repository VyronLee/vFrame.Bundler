//------------------------------------------------------------
//        File:  BundlerGenerator.cs
//       Brief:  BundlerGenerator
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 10:40
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Utils;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.Editor
{
    public class BundlerGenerator
    {
        private static BundlerGenerator _instance;

        private static readonly BuildAssetBundleOptions kAssetBundleBuildOptions =
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.StrictMode;

        public static BundlerGenerator Instance
        {
            get { return _instance = _instance ?? new BundlerGenerator(); }
        }

        private static bool IsUnmanagedResources(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;

            var unmanaged = false;
            unmanaged |= Path.GetExtension(name) == ".meta";
            unmanaged |= Path.GetExtension(name) == ".cs";
            unmanaged |= Path.GetExtension(name) == ".asset";
            unmanaged |= Path.GetExtension(name) == ".dll";
            unmanaged |= name.EndsWith("unity_builtin_extra");
            unmanaged |= name.EndsWith("unity default resources");
            return unmanaged;
        }

        private static bool IsShader(string name) {
            return Path.GetExtension(name) == ".shader";
        }

        private static bool IsExclude(string path, string pattern)
        {
            path = PathUtility.NormalizePath(path);
            return !string.IsNullOrEmpty(pattern) && Regex.IsMatch(path, pattern);
        }

        public void GenerateManifest(BundlerBuildRule buildRule, string outputPath)
        {
            var reservedSharedBundle = new ReservedSharedBundleInfo();
            var depsInfo = ParseBundleDependenciesFromRules(buildRule, ref reservedSharedBundle);
            var bundlesInfo = GenerateBundlesInfo(ref depsInfo, ref reservedSharedBundle);
            var manifest = GenerateBundlerManifest(ref bundlesInfo);
            SaveManifest(ref manifest, outputPath);
        }

        public void GenerateAssetBundles(BundlerManifest manifest, BuildTarget platform)
        {
            var fullPath = PathUtility.Combine(Application.streamingAssetsPath, BundlerDefaultBuildSettings.kBundlePath);
            var outputPath = PathUtility.AbsolutePathToRelativeProjectPath(fullPath);

            var builds = GenerateAssetBundleBuilds(manifest);
            BuildPipeline.BuildAssetBundles(outputPath, builds, kAssetBundleBuildOptions, platform);
        }

        /// <summary>
        ///     Re-generate target asset bundle. Dependencies WON'T be re-generated.
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="path"></param>
        /// <param name="platform"></param>
        /// <param name="outputPath"></param>
        public void RegenerateAssetBundle(BundlerManifest manifest, string path, BuildTarget platform,
            string outputPath)
        {
            var bundleName = PathUtility.NormalizeAssetBundlePath(path);

            var assets = manifest.assets.Where(v => v.Value.bundle == bundleName);

            var build = new AssetBundleBuild
            {
                assetNames = assets.Select(v => v.Value.name).ToArray(),
                assetBundleName = bundleName
            };
            BuildPipeline.BuildAssetBundles(outputPath, new[] {build}, kAssetBundleBuildOptions, platform);
        }

        private DependenciesInfo ParseBundleDependenciesFromRules(BundlerBuildRule buildRule,
            ref ReservedSharedBundleInfo reserved)
        {
            var depsInfo = new DependenciesInfo();
            foreach (var rule in buildRule.rules)
                switch (rule.packType)
                {
                    case PackType.PackByFile:
                        ParseBundleDependenciesByRuleOfPackByFile(rule, ref depsInfo, ref reserved);
                        break;
                    case PackType.PackByDirectory:
                        ParseBundleDependenciesByRuleOfPackByDirectory(rule, ref depsInfo, ref reserved);
                        break;
                    case PackType.PackBySubDirectory:
                        ParseBundleDependenciesByRuleOfPackBySubDirectory(rule, ref depsInfo, ref reserved);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return depsInfo;
        }

        private void ParseBundleDependenciesByRuleOfPackByFile(BundleRule rule, ref DependenciesInfo depsInfo,
            ref ReservedSharedBundleInfo reserved)
        {
            var excludePattern = rule.excludePattern;
            var searchPath = PathUtility.RelativeDataPathToAbsolutePath(rule.path);
            var files = Directory.GetFiles(searchPath, rule.searchPattern, SearchOption.AllDirectories)
                .Where(v => !IsUnmanagedResources(v))
                .Where(v => !IsExclude(v, excludePattern))
                .ToArray();

            var index = 0;
            foreach (var file in files)
            {
                EditorUtility.DisplayProgressBar("Parsing Rule of Pack By File Name", file,
                    (float) index++ / files.Length);

                var bundleName = PathUtility.NormalizeAssetBundlePath(file);
                bundleName = string.Format(BundlerDefaultBuildSettings.kBundleFormatter, bundleName);
                bundleName = BundlerDefaultBuildSettings.kHashAssetBundlePath
                    ? PathUtility.HashPath(bundleName)
                    : bundleName;

                var relativePath = PathUtility.AbsolutePathToRelativeProjectPath(file);
                if (rule.shared)
                    reserved.Add(relativePath, bundleName);

                AddDependenciesInfo(bundleName, relativePath, ref depsInfo, ref reserved);
            }

            EditorUtility.ClearProgressBar();
        }

        private void ParseBundleDependenciesByRuleOfPackByDirectory(BundleRule rule, ref DependenciesInfo depsInfo,
            ref ReservedSharedBundleInfo reserved) {
            var bundleName = PathUtility.NormalizeAssetBundlePath(rule.path);
            bundleName = string.Format(BundlerDefaultBuildSettings.kBundleFormatter, bundleName);
            bundleName = BundlerDefaultBuildSettings.kHashAssetBundlePath
                ? PathUtility.HashPath(bundleName)
                : bundleName;

            var searchPath = PathUtility.RelativeDataPathToAbsolutePath(rule.path);

            var excludePattern = rule.excludePattern;
            var files = Directory.GetFiles(searchPath, rule.searchPattern, SearchOption.AllDirectories)
                .Where(v => !IsUnmanagedResources(v))
                .Where(v => !IsExclude(v, excludePattern))
                .ToArray();

            var index = 0;
            foreach (var file in files)
            {
                EditorUtility.DisplayProgressBar("Parsing Rule of Pack By Directory Name", file,
                    (float) index++ / files.Length);

                var relativePath = PathUtility.AbsolutePathToRelativeProjectPath(file);
                if (rule.shared)
                    reserved.Add(relativePath, bundleName);

                AddDependenciesInfo(bundleName, relativePath, ref depsInfo, ref reserved);
            }

            EditorUtility.ClearProgressBar();
        }

        private void ParseBundleDependenciesByRuleOfPackBySubDirectory(BundleRule rule, ref DependenciesInfo depsInfo,
            ref ReservedSharedBundleInfo reserved) {
            var excludePattern = rule.excludePattern;

            var searchPath = PathUtility.RelativeDataPathToAbsolutePath(rule.path);
            var subDirectories = Directory.GetDirectories(searchPath, "*.*", (SearchOption) rule.depth)
                .Where(v => !IsExclude(v, excludePattern))
                .ToArray();

            foreach (var subDirectory in subDirectories)
            {
                var files = Directory
                    .GetFiles(subDirectory, rule.searchPattern, SearchOption.AllDirectories)
                    .Where(v => !IsUnmanagedResources(v))
                    .Where(v => !IsExclude(v, excludePattern))
                    .ToArray();

                var bundleName = PathUtility.NormalizeAssetBundlePath(subDirectory);
                bundleName = string.Format(BundlerDefaultBuildSettings.kBundleFormatter, bundleName);
                bundleName = BundlerDefaultBuildSettings.kHashAssetBundlePath
                    ? PathUtility.HashPath(bundleName)
                    : bundleName;

                var index = 0;
                foreach (var file in files)
                {
                    EditorUtility.DisplayProgressBar("Parsing Rule of Pack By Sub Directory Name", file,
                        (float) index++ / files.Length);

                    var relativePath = PathUtility.AbsolutePathToRelativeProjectPath(file);
                    if (rule.shared)
                        reserved.Add(relativePath, bundleName);

                    AddDependenciesInfo(bundleName, relativePath, ref depsInfo, ref reserved);
                }

                EditorUtility.ClearProgressBar();
            }
        }

        private void AddDependenciesInfo(string bundleName, string relativePath, ref DependenciesInfo info,
            ref ReservedSharedBundleInfo reserved)
        {
            var asset = AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
            var dependencies = EditorUtility.CollectDependencies(new[] {asset})
                .Select(AssetDatabase.GetAssetPath)
                .Where(v => !IsUnmanagedResources(v))
                .Distinct()
                .ToList();
            dependencies.Add(relativePath);

            foreach (var dependency in dependencies)
            {
                if (!info.ContainsKey(dependency))
                    info[dependency] = new DependencyInfo();

                if (IsShader(dependency)) {
                    if (BundlerDefaultBuildSettings.kSeparateShaderBundle) {
                        info[dependency].referenceInBundles.Add(BundlerDefaultBuildSettings.kSeparatedShaderBundleName);
                        reserved[dependency] = BundlerDefaultBuildSettings.kSeparatedShaderBundleName;
                    }
                }
                info[dependency].referenceInBundles.Add(bundleName);
            }
        }

        private BundlesInfo GenerateBundlesInfo(ref DependenciesInfo depsInfo, ref ReservedSharedBundleInfo reserved)
        {
            var sharedBundles = ParseSharedBundleInfo(ref depsInfo, ref reserved);
            var noneSharedBundles = ParseNoneSharedBundleInfo(ref depsInfo, ref sharedBundles);

            var bundles = new BundlesInfo();
            sharedBundles.Concat(noneSharedBundles).ToList().ForEach(kv => bundles.Add(kv.Key, kv.Value));
            return bundles;
        }

        private BundlesInfo ParseSharedBundleInfo(ref DependenciesInfo depsInfo, ref ReservedSharedBundleInfo reserved)
        {
            var sharedDict = new Dictionary<HashSet<string>, string>(new StringHashSetComparer());

            var index = 0;
            // Determine shared bundles
            foreach (var kv in depsInfo)
            {
                var asset = kv.Key;

                // Ignore this asset when forced build in shared bundle.
                if (reserved.ContainsKey(asset))
                    continue;

                var depSet = kv.Value;

                // The count of dependencies no greater than 1 means that there are no other bundles
                // sharing this asset
                if (depSet.referenceInBundles.Count <= 1)
                    continue;

                // Otherwise, assets which depended by the same bundles will be separated to shared bundle.
                if (BundlerDefaultBuildSettings.kCombineSharedAssets) {
                    if (sharedDict.ContainsKey(depSet.referenceInBundles))
                    {
                        depSet.bundle = sharedDict[depSet.referenceInBundles];
                        continue;
                    }
                    sharedDict.Add(depSet.referenceInBundles,
                        depSet.bundle = string.Format(BundlerDefaultBuildSettings.kSharedBundleFormatter,
                            (++index).ToString()));
                }
                else {
                    depSet.bundle = string.Format(BundlerDefaultBuildSettings.kSharedBundleFormatter,
                        (++index).ToString());
                }
            }

            // Collect shared bundles info
            var bundlesInfo = new BundlesInfo();
            foreach (var kv in depsInfo)
            {
                var name = kv.Value.bundle;
                if (string.IsNullOrEmpty(name))
                    continue;

                if (!bundlesInfo.ContainsKey(name))
                    bundlesInfo.Add(name, new BundleInfo());
                bundlesInfo[name].assets.Add(kv.Key);
            }

            // Generate unique bundle name according to assets list.
            var sharedBundle = new BundlesInfo();
            foreach (var kv in bundlesInfo)
            {
                var assets = kv.Value.assets.ToList();
                assets.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

                var name = string.Join("-", assets.ToArray());
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(name)))
                {
                    var nameHash = CalculateMd5(stream);
                    var uniqueName = string.Format(BundlerDefaultBuildSettings.kSharedBundleFormatter, nameHash);

                    var bundleInfo = new BundleInfo();
                    foreach (var asset in kv.Value.assets)
                        bundleInfo.assets.Add(asset);
                    sharedBundle.Add(uniqueName, bundleInfo);
                }
            }

            // Add reserved shared bundles info
            foreach (var kv in reserved)
            {
                var assetName = kv.Key;
                var bundle = kv.Value;
                if (!sharedBundle.ContainsKey(bundle))
                    sharedBundle.Add(bundle, new BundleInfo());
                sharedBundle[bundle].assets.Add(assetName);
            }

            return sharedBundle;
        }

        private BundlesInfo ParseNoneSharedBundleInfo(ref DependenciesInfo depsInfo, ref BundlesInfo sharedBundles)
        {
            // Parse none shared bundle info
            var noneSharedBundles = new BundlesInfo();
            foreach (var kv in depsInfo)
            {
                var assetName = kv.Key;
                var bundles = kv.Value.referenceInBundles;

                var sharedBundle = sharedBundles.FirstOrDefault(skv => skv.Value.assets.Contains(assetName)).Key ?? "";

                foreach (var bundle in bundles)
                {
                    if (!string.IsNullOrEmpty(sharedBundles.FirstOrDefault(skv => skv.Key == bundle).Key))
                        continue; // Equals to shared bundle, pass.

                    if (!noneSharedBundles.ContainsKey(bundle))
                        noneSharedBundles.Add(bundle, new BundleInfo());

                    if (!string.IsNullOrEmpty(sharedBundle))
                        noneSharedBundles[bundle].dependencies.Add(sharedBundle);
                    else
                        noneSharedBundles[bundle].assets.Add(assetName);
                }
            }

            // Move *.unity files to independent bundles, because 'Cannot mark assets and scenes in one AssetBundle'
            var sceneBundles = new BundlesInfo();
            foreach (var kv in noneSharedBundles)
            {
                var noneSharedBundleName = kv.Key;
                var noneSharedBundleInfo = kv.Value;
                var noneSharedAssets = kv.Value.assets;
                var removed = new List<string>();
                var separatedBundles = new List<BundleInfo>();
                foreach (var asset in noneSharedAssets)
                {
                    if (!asset.EndsWith(".unity"))
                        continue;

                    string sceneBundleName;
                    sceneBundleName = string.Format(BundlerDefaultBuildSettings.kSceneBundleFormatter,
                        PathUtility.RelativeProjectPathToAbsolutePath(asset));
                    sceneBundleName = PathUtility.NormalizeAssetBundlePath(sceneBundleName);

                    if (sceneBundles.ContainsKey(sceneBundleName))
                        throw new BundleException("Scene asset bundle duplicated: " + sceneBundleName);

                    var bundle = new BundleInfo();
                    bundle.assets.Add(asset);
                    bundle.dependencies.Add(noneSharedBundleName);

                    sceneBundles.Add(sceneBundleName, bundle);
                    separatedBundles.Add(bundle);

                    removed.Add(asset);
                }

                removed.ForEach(v => noneSharedAssets.Remove(v));

                // Clear scene bundles' dependencies if none-shared assets list come into empty after separated.
                if (noneSharedAssets.Count <= 0)
                    separatedBundles.ForEach(bundle =>
                    {
                        bundle.dependencies.Clear();
                        foreach (var dependency in noneSharedBundleInfo.dependencies)
                            bundle.dependencies.Add(dependency);
                    });
            }

            // Merge to none shared bundles
            foreach (var kv in sceneBundles)
                noneSharedBundles.Add(kv.Key, kv.Value);

            return noneSharedBundles;
        }

        private BundlerManifest GenerateBundlerManifest(ref BundlesInfo bundlesInfo)
        {
            var manifest = new BundlerManifest();

            foreach (var kv in bundlesInfo)
            {
                var bundleName = kv.Key;
                var assets = kv.Value.assets;
                var dependencies = kv.Value.dependencies;

                // No assets in this bundle, skips.
                if (assets.Count <= 0)
                    continue;

                // Generate assets data
                foreach (var asset in assets)
                {
                    if (manifest.assets.ContainsKey(asset))
                        throw new BundleException(string.Format("Asset duplicated: {0}", asset));

                    var assetData = new AssetData
                    {
                        name = asset,
                        bundle = bundleName
                    };
                    manifest.assets.Add(asset, assetData);
                }

                // Generate bundles data
                if (manifest.bundles.ContainsKey(bundleName))
                    throw new BundleException(string.Format("Bundle duplicated: {0}", bundleName));

                manifest.bundles.Add(bundleName, new BundleData {name = bundleName});
                foreach (var depName in dependencies)
                    manifest.bundles[bundleName].dependencies.Add(depName);
            }

            return manifest;
        }

        private void SaveManifest(ref BundlerManifest manifest, string outputPath)
        {
            var fullPath = PathUtility.Combine(Application.streamingAssetsPath, outputPath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var jsonData = JsonUtility.ToJson(manifest);
            File.WriteAllText(fullPath, jsonData);
        }

        private AssetBundleBuild[] GenerateAssetBundleBuilds(BundlerManifest manifest)
        {
            var bundles = new Dictionary<string, HashSet<string>>();

            foreach (var kv in manifest.assets)
            {
                if (!bundles.ContainsKey(kv.Value.bundle))
                    bundles.Add(kv.Value.bundle, new HashSet<string>());

                bundles[kv.Value.bundle].Add(kv.Value.name);
            }

            var builds = new List<AssetBundleBuild>();
            foreach (var kv in bundles)
            {
                if (kv.Value.Count <= 0)
                    continue;

                var build = new AssetBundleBuild
                {
                    assetBundleName = kv.Key,
                    assetNames = kv.Value.ToArray()
                };
                builds.Add(build);
            }

            return builds.ToArray();
        }

        private string CalculateMd5(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        // HashSet<T> EqualityComparer does not implement in mono 2.0
        private class StringHashSetComparer : IEqualityComparer<HashSet<string>>
        {
            public bool Equals(HashSet<string> x, HashSet<string> y)
            {
                return !x.Any(v => !y.Contains(v)) && !y.Any(v => !x.Contains(v));
            }

            public int GetHashCode(HashSet<string> obj)
            {
                var hashcode = 0x7FFFFFFF;
                foreach (var s in obj) hashcode |= s.GetHashCode();

                return hashcode;
            }
        }

        #region Data Structure Alias

        // Asset name -> bundles name contains this asset
        private class DependenciesInfo : Dictionary<string, DependencyInfo>
        {
        }

        private class DependencyInfo
        {
            public readonly HashSet<string> referenceInBundles = new HashSet<string>();
            public string bundle;
        }

        // Bundle name -> assets in this bundle + bundle dependencies
        private class BundlesInfo : Dictionary<string, BundleInfo>
        {
        }

        private class BundleInfo
        {
            public readonly HashSet<string> assets = new HashSet<string>();
            public readonly HashSet<string> dependencies = new HashSet<string>();
        }

        // Asset name -> bundle name
        private class ReservedSharedBundleInfo : Dictionary<string, string>
        {
        }

        #endregion
    }
}