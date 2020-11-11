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
using vFrame.Bundler.Utils.Pools;
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

        private static bool IsScriptableObject(string name) {
            return Path.GetExtension(name) == ".asset";
        }

        private static bool IsScript(string name) {
            return Path.GetExtension(name) == ".cs";
        }

        private static bool IsAssembly(string name) {
            return Path.GetExtension(name) == ".dll";
        }

        private static bool IsBuiltinResource(string name) {
            return name.EndsWith("unity_builtin_extra")
                || name.EndsWith("unity default resources");
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
            ResolveBundleDependencies(ref bundlesInfo);
            var manifest = GenerateBundlerManifest(ref bundlesInfo);
            SaveManifest(ref manifest, outputPath);
            Resources.UnloadUnusedAssets();
        }

        public void StripUnmanagedFiles(BundlerBuildRule buildRule, BundlerManifest manifest) {
            var managedFiles = GetManagedFilesByRules(buildRule);
            var toRemoved = new List<string>();
            foreach (var kv in manifest.assets) {
                if (!managedFiles.Contains(kv.Key)) {
                    toRemoved.Add(kv.Key);
                }
            }
            toRemoved.ForEach(v => manifest.assets.Remove(v));
            Debug.Log(string.Format("{0} unmanaged files removed from manifest.", toRemoved.Count));
        }

        public void GenerateAssetBundles(BundlerManifest manifest, BuildTarget platform) {
            var outputPath = PathUtility.Combine(Application.streamingAssetsPath, BundlerBuildSettings.kBundlePath);
            Debug.Log(string.Format("Generate asset bundles to path: {0}", outputPath));

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
                assetNames = assets.Select(v => v.Key).ToArray(),
                assetBundleName = bundleName
            };
            BuildPipeline.BuildAssetBundles(outputPath, new[] {build}, kAssetBundleBuildOptions, platform);
        }

        /// <summary>
        /// Validate bundle dependencies
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="outputPath"></param>
        /// <exception cref="BundleException"></exception>
        public void ValidateBundleDependencies(BundlerManifest manifest, string outputPath) {
            var abManifestPath = string.Format("{0}/{1}", outputPath, new DirectoryInfo(outputPath).Name);
            var abManifestRelativePath = PathUtility.AbsolutePathToRelativeProjectPath(abManifestPath);
            var ab = AssetBundle.LoadFromFile(abManifestRelativePath);
            if (!ab) {
                throw new BundleException("Load asset bundle failed: " + abManifestPath);
            }

            var abManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (!abManifest) {
                throw new BundleException("Load asset bundle manifest failed: " + abManifestPath);
            }

            void ValidateBundle(string assetBundle, List<string> validated) {
                if (validated.Contains(assetBundle)) {
                    throw new BundleException("Circular dependency detected: " + assetBundle);
                }
                validated.Add(assetBundle);

                if (!manifest.bundles.ContainsKey(assetBundle)) {
                    throw new BundleException("Bundle does not contain in manifest: " + assetBundle);
                }

                var dependencies = abManifest.GetDirectDependencies(assetBundle);
                if (dependencies.Length != manifest.bundles[assetBundle].dependencies.Count) {
                    throw new BundleException("Bundle dependencies count not match: " + assetBundle);
                }

                foreach (var dependency in dependencies) {
                    if (!manifest.bundles[assetBundle].dependencies.Contains(dependency)) {
                        throw new BundleException(string.Format(
                            "Bundle dependencies does not equal: {0}, lost dependency: {1}", assetBundle, dependency));
                    }
                    Debug.Log("Validating dependency: " + dependency);
                    ValidateBundle(dependency, validated);
                }
                validated.Remove(assetBundle);
            }

            var index = 0f;
            var abs = abManifest.GetAllAssetBundles();
            try {
                foreach (var assetBundle in abs) {
                    EditorUtility.DisplayProgressBar("Validating asset bundle", assetBundle, index++/abs.Length);
                    Debug.Log("Validating asset bundle: " + assetBundle);
                    ValidateBundle(assetBundle, new List<string>());
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
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

            try {
                var index = 0;
                foreach (var file in files)
                {
                    EditorUtility.DisplayProgressBar("Parsing Rule of Pack By File Name", file,
                        (float) index++ / files.Length);

                    var bundleName = PathUtility.NormalizeAssetBundlePath(file);
                    bundleName = string.Format(BundlerBuildSettings.kBundleFormatter, bundleName);
                    bundleName = BundlerBuildSettings.kHashAssetBundlePath
                        ? PathUtility.HashPath(bundleName)
                        : bundleName;

                    var relativePath = PathUtility.AbsolutePathToRelativeProjectPath(file);
                    if (rule.shared)
                        TryAddToForceSharedBundle(relativePath, bundleName, ref reserved);

                    AddDependenciesInfo(bundleName, relativePath, ref depsInfo, ref reserved);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        private void ParseBundleDependenciesByRuleOfPackByDirectory(BundleRule rule, ref DependenciesInfo depsInfo,
            ref ReservedSharedBundleInfo reserved) {
            var bundleName = PathUtility.NormalizeAssetBundlePath(rule.path);
            bundleName = string.Format(BundlerBuildSettings.kBundleFormatter, bundleName);
            bundleName = BundlerBuildSettings.kHashAssetBundlePath
                ? PathUtility.HashPath(bundleName)
                : bundleName;

            var searchPath = PathUtility.RelativeDataPathToAbsolutePath(rule.path);

            var excludePattern = rule.excludePattern;
            var files = Directory.GetFiles(searchPath, rule.searchPattern, SearchOption.AllDirectories)
                .Where(v => !IsUnmanagedResources(v))
                .Where(v => !IsExclude(v, excludePattern))
                .ToArray();

            try {
                var index = 0;
                foreach (var file in files)
                {
                    EditorUtility.DisplayProgressBar("Parsing Rule of Pack By Directory Name", file,
                        (float) index++ / files.Length);

                    var relativePath = PathUtility.AbsolutePathToRelativeProjectPath(file);
                    if (rule.shared)
                        TryAddToForceSharedBundle(relativePath, bundleName, ref reserved);

                    AddDependenciesInfo(bundleName, relativePath, ref depsInfo, ref reserved);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
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
                bundleName = string.Format(BundlerBuildSettings.kBundleFormatter, bundleName);
                bundleName = BundlerBuildSettings.kHashAssetBundlePath
                    ? PathUtility.HashPath(bundleName)
                    : bundleName;

                try {
                    var index = 0;
                    foreach (var file in files)
                    {
                        EditorUtility.DisplayProgressBar("Parsing Rule of Pack By Sub Directory Name", file,
                            (float) index++ / files.Length);

                        var relativePath = PathUtility.AbsolutePathToRelativeProjectPath(file);
                        if (rule.shared)
                            TryAddToForceSharedBundle(relativePath, bundleName, ref reserved);

                        AddDependenciesInfo(bundleName, relativePath, ref depsInfo, ref reserved);
                    }
                }
                finally {
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        private string[] GetManagedFilesByRules(BundlerBuildRule rules) {
            var files = new List<string>();
            rules.rules.ForEach(v => files.AddRange(GetFileListByRule(v)));
            return files.ToArray();
        }

        private string[] GetFileListByRule(BundleRule rule) {
            var excludePattern = rule.excludePattern;
            var searchPath = PathUtility.RelativeDataPathToAbsolutePath(rule.path);
            switch (rule.packType) {
                case PackType.PackByFile:
                case PackType.PackByDirectory: {
                    return Directory.GetFiles(searchPath, rule.searchPattern, SearchOption.AllDirectories)
                        .Where(v => !IsUnmanagedResources(v))
                        .Where(v => !IsExclude(v, excludePattern))
                        .Select(v => PathUtility.AbsolutePathToRelativeProjectPath(v))
                        .ToArray();
                }
                case PackType.PackBySubDirectory: {
                    var subDirectories = Directory.GetDirectories(searchPath, "*.*", (SearchOption) rule.depth)
                        .Where(v => !IsExclude(v, excludePattern))
                        .ToArray();

                    var files = new List<string>();
                    foreach (var subDirectory in subDirectories) {
                        var filesInDir = Directory
                            .GetFiles(subDirectory, rule.searchPattern, SearchOption.AllDirectories)
                            .Where(v => !IsUnmanagedResources(v))
                            .Where(v => !IsExclude(v, excludePattern))
                            .Select(v => PathUtility.AbsolutePathToRelativeProjectPath(v));
                        files.AddRange(filesInDir);
                    }
                    return files.ToArray();
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TryAddToForceSharedBundle(string assetName, string bundleName, ref ReservedSharedBundleInfo reserved) {
            if (reserved.ContainsKey(assetName)) {
                throw new BundleRuleConflictException(
                    string.Format("Asset({0} has already contain in bundle: {1}, but trying add to new bundle: {2})",
                        assetName, reserved[assetName], bundleName));
            }
            reserved.Add(assetName, bundleName);
        }

        private void AddDependenciesInfo(string bundleName, string relativePath, ref DependenciesInfo info,
            ref ReservedSharedBundleInfo reserved)
        {
            var dependencies = CollectDependencies(relativePath);
            var deps = dependencies.ToList();
            deps.Add(relativePath);

            foreach (var dependency in deps)
            {
                if (!info.ContainsKey(dependency))
                    info[dependency] = new DependencyInfo();

                if (IsShader(dependency)) {
                    if (BundlerBuildSettings.kSeparateShaderBundle) {
                        info[dependency].referenceInBundles.Add(BundlerBuildSettings.kSeparatedShaderBundleName);
                        reserved[dependency] = BundlerBuildSettings.kSeparatedShaderBundleName;
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
            sharedBundles.Concat(noneSharedBundles).ToList().ForEach(kv => {
                if (kv.Value.assets.Count > 0) {
                    bundles.Add(kv.Key, kv.Value);
                }
            });
            return bundles;
        }

        private static IEnumerable<string> CollectDependencies(string asset) {
            var assetObj = AssetDatabase.LoadAssetAtPath<Object>(asset);
            var dep1 = EditorUtility.CollectDependencies(new[] {assetObj});
            var dep2 = dep1.Select(AssetDatabase.GetAssetPath).ToArray();
            var dep3 = dep2.Where(v => v != asset).ToArray();
            var dep4 = dep3.Where(v => !IsUnmanagedResources(v)).ToArray();
            var dep5 = dep4.Distinct().ToArray();
            return dep5;
        }

        private void ResolveBundleDependencies(ref BundlesInfo bundlesInfo) {
            var info = bundlesInfo;
            foreach (var kv in bundlesInfo) {
                //Debug.Log(string.Format("Resolving bundle dependency: {0}", kv.Key));

                var bundleInfo = kv.Value;
                bundleInfo.dependencies.Clear();

                void ResolveAssetDependency(string asset) {
                    var dependencies = CollectDependencies(asset);
                    foreach (var dependencyAsset in dependencies) {
                        // Which bundle contains this asset?
                        var depBundle = info.FirstOrDefault(
                            v => v.Value.assets.Contains(dependencyAsset)).Key;

                        if (string.IsNullOrEmpty(depBundle)) {
                            continue;
                        }

                        if (depBundle == kv.Key) {
                            continue;
                        }
                        bundleInfo.dependencies.Add(depBundle);
                    }
                }

                foreach (var asset in bundleInfo.assets) {
                    ResolveAssetDependency(asset);
                }
            }

            // Filter out redundant dependencies.
            var bsInfo = bundlesInfo;
            foreach (var kv in bundlesInfo) {
                var bundleInfo = kv.Value;

                bool IsDependencyInBundle(string bundleName, BundleInfo bInfo) {
                    foreach (var dependency in bInfo.dependencies) {
                        if (dependency == bundleName) {
                            return true;
                        }

                        if (!bsInfo.ContainsKey(dependency))
                            continue;

                        if (IsDependencyInBundle(bundleName, bsInfo[dependency])) {
                            return true;
                        }
                    }
                    return false;
                }

                bool IsReferenceInDependencies(string toCheck, BundleInfo bInfo) {
                    //Debug.Log(string.Format("Checking dependency: {0}", toCheck));
                    foreach (var dependency in bInfo.dependencies) {
                        if (dependency == toCheck) {
                            continue;
                        }

                        if (!bsInfo.ContainsKey(dependency))
                            continue;

                        if (IsDependencyInBundle(toCheck, bsInfo[dependency])) {
                            return true;
                        }
                    }
                    return false;
                }

                //Debug.Log(string.Format("Checking redundant dependency: {0}", kv.Key));

                var toRemove = ListPool<string>.Get();
                foreach (var dependency in bundleInfo.dependencies) {
                    if (IsReferenceInDependencies(dependency, bundleInfo)) {
                        toRemove.Add(dependency);
                    }
                }
                toRemove.ForEach(v => {
                    //Debug.Log(string.Format("Dependency redundant, removed: {0}, in bundle: {1}", v, kv.Key));
                    bundleInfo.dependencies.Remove(v);
                });
                ListPool<string>.Return(toRemove);
            }
        }

        private BundlesInfo ParseSharedBundleInfo(ref DependenciesInfo depsInfo, ref ReservedSharedBundleInfo reserved)
        {
            var sharedDict = new Dictionary<string, string>();

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
                if (!sharedDict.ContainsKey(asset)) {
                    sharedDict[asset] = string.Format(BundlerBuildSettings.kSharedBundleFormatter,
                        (++index).ToString());

                    // Sub-assets dependencies.
                    var deps = CollectDependencies(asset);
                    foreach (var dep in deps) {
                        if (reserved.ContainsKey(dep))
                            continue;
                        sharedDict[dep] = string.Format(BundlerBuildSettings.kSharedBundleFormatter,
                            (++index).ToString());
                    }
                }
            }

            // Collect shared bundles info
            var bundlesInfo = new BundlesInfo();
            foreach (var kv in sharedDict) {
                var name = sharedDict[kv.Key];
                if (bundlesInfo.ContainsKey(name))
                    throw new BundleException("Shared bundle duplicated: " + name);

                bundlesInfo[name] = new BundleInfo();
                bundlesInfo[name].assets.Add(kv.Key);
            }

            // Generate unique bundle name according to assets list.
            var sharedBundle = new BundlesInfo();
            foreach (var kv in bundlesInfo)
            {
                var assets = kv.Value.assets.ToList();
                assets.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

                var bundleName = string.Join("-", assets.ToArray());
                if (BundlerBuildSettings.kHashSharedBundle) {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(bundleName)))
                    {
                        var nameHash = CalculateMd5(stream);
                        bundleName = string.Format(BundlerBuildSettings.kSharedBundleFormatter, nameHash);
                    }
                }
                else {
                    bundleName = string.Format(BundlerBuildSettings.kSharedBundleFormatter, bundleName.ToLower());
                }

                var bundleInfo = new BundleInfo();
                foreach (var asset in kv.Value.assets)
                    bundleInfo.assets.Add(asset);
                sharedBundle.Add(bundleName, bundleInfo);
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

                    if (string.IsNullOrEmpty(sharedBundle))
                        noneSharedBundles[bundle].assets.Add(assetName);
                }
            }

            // Move *.unity files to independent bundles, because 'Cannot mark assets and scenes in one AssetBundle'
            var sceneBundles = new BundlesInfo();
            foreach (var kv in noneSharedBundles)
            {
                var noneSharedAssets = kv.Value.assets;
                var removed = new List<string>();
                foreach (var asset in noneSharedAssets)
                {
                    if (!asset.EndsWith(".unity"))
                        continue;

                    var sceneBundleName = string.Format(BundlerBuildSettings.kSceneBundleFormatter,
                        PathUtility.RelativeProjectPathToAbsolutePath(asset));
                    sceneBundleName = PathUtility.NormalizeAssetBundlePath(sceneBundleName);

                    if (sceneBundles.ContainsKey(sceneBundleName))
                        throw new BundleException("Scene asset bundle duplicated: " + sceneBundleName);

                    var bundle = new BundleInfo();
                    bundle.assets.Add(asset);

                    sceneBundles.Add(sceneBundleName, bundle);

                    removed.Add(asset);
                }

                removed.ForEach(v => noneSharedAssets.Remove(v));
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
                        throw new BundleException(string.Format(
                            "Asset duplicated: {0}, already reference in bundle: {1}, conflict with: {2}",
                            asset, manifest.assets[asset].bundle, bundleName));

                    var assetData = new AssetData
                    {
                        //name = asset,
                        bundle = bundleName
                    };
                    manifest.assets.Add(asset, assetData);
                }

                // Generate bundles data
                if (manifest.bundles.ContainsKey(bundleName))
                    throw new BundleException(string.Format("Bundle duplicated: {0}", bundleName));

                manifest.bundles.Add(bundleName, new BundleData()); // {name = bundleName});
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

                bundles[kv.Value.bundle].Add(kv.Key);
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