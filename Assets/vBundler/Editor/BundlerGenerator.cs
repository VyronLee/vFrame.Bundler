using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using vBundler.Utils;

namespace vBundler.Editor
{
    public class BundlerGenerator
    {
        private static BundlerGenerator _instance;

        public static BundlerGenerator Instance
        {
            get { return _instance = _instance ?? new BundlerGenerator(); }
        }

        public void GenerateManifest(BundlerBuildRule buildRule, string outputPath)
        {
            var depsInfo = ParseBundleDependenciesFromRules(buildRule);
            var bundlesInfo = GenerateBundlesInfo(ref depsInfo);
            var manifest = GenerateBundlerManifest(ref bundlesInfo);
            SaveManifest(ref manifest, outputPath);
        }

        public void GenerateAssetBundles(BundlerManifest manifest, BuildTarget platform)
        {
            var fullPath = PathUtility.RelativeDataPathToFullPath(BundlerSetting.kDefaultBundlePath);
            var outputPath = PathUtility.FullPathToRelativeProjectPath(fullPath);

            var builds = GenerateAssetBundleBuilds(manifest);
            BuildPipeline.BuildAssetBundles(outputPath, builds,
                BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode, platform);
        }

        private DependenciesInfo ParseBundleDependenciesFromRules(BundlerBuildRule buildRule)
        {
            var depsInfo = new DependenciesInfo();
            foreach (var rule in buildRule.rules)
                switch (rule.packType)
                {
                    case PackType.PackByDirectory:
                    {
                        var bundleName = PathUtility.NormalizeAssetBundlePath(rule.path);
                        var searchPath = PathUtility.RelativeDataPathToFullPath(rule.path);

                        var files = Directory.GetFiles(searchPath, rule.searchPattern, SearchOption.AllDirectories)
                            .Where(v => Path.GetExtension(v) != ".meta")
                            .Where(v => Path.GetExtension(v) != ".cs");

                        var index = 0;
                        foreach (var file in files)
                        {
                            EditorUtility.DisplayProgressBar("Parsing Rule of Pack By Directory Name", file,
                                (float) index++ / files.Count());

                            AddDependenciesInfo(bundleName, file, ref depsInfo);
                        }

                        EditorUtility.ClearProgressBar();

                        break;
                    }
                    case PackType.PackByFile:
                    {
                        var searchPath = PathUtility.RelativeDataPathToFullPath(rule.path);
                        var files = Directory.GetFiles(searchPath, rule.searchPattern, SearchOption.AllDirectories);

                        var index = 0;
                        foreach (var file in files)
                        {
                            EditorUtility.DisplayProgressBar("Parsing Rule of Pack By File Name", file,
                                (float) index++ / files.Length);

                            var bundleName = PathUtility.NormalizeAssetBundlePath(file);
                            AddDependenciesInfo(bundleName, file, ref depsInfo);
                        }

                        EditorUtility.ClearProgressBar();

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return depsInfo;
        }

        private void AddDependenciesInfo(string bundleName, string fileFullPath, ref DependenciesInfo info)
        {
            var relativePath = PathUtility.FullPathToRelativeProjectPath(fileFullPath);
            var dependencies = AssetDatabase.GetDependencies(relativePath).Where(v => Path.GetExtension(v) != ".cs");

            foreach (var dependency in dependencies)
            {
                if (!info.ContainsKey(dependency))
                    info[dependency] = new DependencyInfo();
                info[dependency].dependencies.Add(bundleName);
            }
        }

        private BundlesInfo GenerateBundlesInfo(ref DependenciesInfo depsInfo)
        {
            var sharedBundles = ParseSharedBundleInfo(ref depsInfo);
            var noneSharedBundles = ParseNoneSharedBundleInfo(ref depsInfo, ref sharedBundles);

            var bundles = new BundlesInfo();
            sharedBundles.Concat(noneSharedBundles).ToList().ForEach(kv => bundles.Add(kv.Key, kv.Value));
            return bundles;
        }

        private BundlesInfo ParseSharedBundleInfo(ref DependenciesInfo depsInfo)
        {
            var sharedDict = new Dictionary<HashSet<string>, string>(new StringHashSetComparer());

            var index = 0;
            // Determine shared bundles name
            foreach (var kv in depsInfo)
            {
                var depSet = kv.Value;

                // the count of dependencies no greater than 1 means that there are no other bundles
                // sharing this asset
                if (depSet.dependencies.Count <= 1)
                    continue;

                // otherwise, assets which depended by the same bundles will be combine into one bundle.
                if (sharedDict.ContainsKey(depSet.dependencies))
                {
                    depSet.target = sharedDict[depSet.dependencies];
                    continue;
                }

                sharedDict.Add(depSet.dependencies,
                    depSet.target = string.Format(BundlerSetting.kDefaultSharedBundleFormatter, (++index).ToString()));
            }

            // Collect shared bundles info
            var sharedBundles = new BundlesInfo();
            foreach (var kv in depsInfo)
            {
                var name = kv.Value.target;

                if (string.IsNullOrEmpty(name))
                    continue;

                if (!sharedBundles.ContainsKey(name))
                    sharedBundles.Add(name, new BundleInfo());
                sharedBundles[name].assets.Add(kv.Key);
            }

            return sharedBundles;
        }

        private BundlesInfo ParseNoneSharedBundleInfo(ref DependenciesInfo depsInfo, ref BundlesInfo sharedBundles)
        {
            var noneSharedBundles = new BundlesInfo();
            foreach (var kv in depsInfo)
            {
                var assetName = kv.Key;
                var bundles = kv.Value.dependencies;

                var sharedBundle = sharedBundles.FirstOrDefault(skv => skv.Value.assets.Contains(assetName)).Key ?? "";

                foreach (var bundle in bundles)
                {
                    if (!noneSharedBundles.ContainsKey(bundle))
                        noneSharedBundles.Add(bundle, new BundleInfo());

                    if (!string.IsNullOrEmpty(sharedBundle))
                        noneSharedBundles[bundle].dependencies.Add(sharedBundle);
                    else
                        noneSharedBundles[bundle].assets.Add(assetName);
                }
            }

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

                // Generate assets data
                foreach (var asset in assets)
                {
                    if (manifest.assets.ContainsKey(asset))
                        throw new InvalidProgramException(string.Format("Asset duplicated: {0}", asset));

                    var assetData = new AssetData
                    {
                        name = asset,
                        bundle = bundleName
                    };
                    manifest.assets.Add(asset, assetData);
                }

                // Generate bundles data
                if (manifest.bundles.ContainsKey(bundleName))
                    throw new InvalidProgramException(string.Format("Bundle duplicated: {0}", bundleName));

                manifest.bundles.Add(bundleName, new BundleData {name = bundleName});
                foreach (var depName in dependencies) manifest.bundles[bundleName].dependencies.Add(depName);
            }

            return manifest;
        }

        private void SaveManifest(ref BundlerManifest manifest, string outputPath)
        {
            var fullPath = PathUtility.RelativeDataPathToFullPath(outputPath);
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
                var build = new AssetBundleBuild
                {
                    assetBundleName = kv.Key,
                    assetNames = kv.Value.ToArray()
                };
                builds.Add(build);
            }

            return builds.ToArray();
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
            public readonly HashSet<string> dependencies = new HashSet<string>();
            public string target;
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

        #endregion
    }
}