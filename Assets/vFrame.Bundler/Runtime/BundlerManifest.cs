﻿//------------------------------------------------------------
//        File:  BundlerManifest.cs
//       Brief:  BundlerManifest
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:19
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace vFrame.Bundler
{
    [Serializable]
    public class BundlerManifest : ISerializationCallbackReceiver
    {
        [NonSerialized] public AssetsTable Assets = new AssetsTable();
        [NonSerialized] public BundlesTable Bundles = new BundlesTable();

        public static BundlerManifest FromJson(string jsonData) {
            if (string.IsNullOrEmpty(jsonData))
                throw new BundleArgumentException($"Argument '{nameof(jsonData)}' cannot be null or empty.");
            return JsonUtility.FromJson<BundlerManifest>(jsonData);
        }

        //=============================================
        // ISerializationCallbackReceiver
        //=============================================

        [SerializeField] private List<string> _assets = new List<string>();
        [SerializeField] private List<int> _assetContainsInBundle = new List<int>();

        [SerializeField] private List<string> _bundles = new List<string>();
        [SerializeField] private List<BundleDependencySetInt> _bundleDependencies = new List<BundleDependencySetInt>();

        public void OnBeforeSerialize() {
            _assets = Assets.Keys.ToList();
            _assets.Sort();

            _bundles = Bundles.Keys.ToList();
            _bundles.Sort();

            _assetContainsInBundle = _assets.Select(GetBundleIndex).ToList();
            _bundleDependencies = _bundles.Select(GetDependencySetInt).ToList();
            return;

            int GetBundleIndex(string path) {
                return _bundles.IndexOf(Assets[path]);
            }

            BundleDependencySetInt GetDependencySetInt(string path) {
                return new BundleDependencySetInt(Bundles[path].Values.Select(v => _bundles.IndexOf(v)));
            }
        }

        public void OnAfterDeserialize() {
            for (var i = 0; i < _assets.Count && i < _assetContainsInBundle.Count; i++) {
                var assetPath = _assets[i];
                var bundlePath = string.Empty;
                var bundleIdx = _assetContainsInBundle[i];
                if (bundleIdx >= 0 && bundleIdx < _bundles.Count) {
                    bundlePath = _bundles[bundleIdx];
                }
                Assets.Add(assetPath, bundlePath);
            }

            for (var i = 0; i < _bundles.Count && i < _bundleDependencies.Count; i++) {
                var bundlePath = _bundles[i];
                var dependenciesInt = _bundleDependencies[i].Values;
                var dependencies = new BundleDependencySet(dependenciesInt.Select(v => _bundles[v]));
                Bundles.Add(bundlePath, dependencies);
            }
        }
    }

    /// <summary>
    /// Asset path => bundle path
    /// </summary>
    public class AssetsTable : Dictionary<string, string>
    {
    }

    /// <summary>
    /// Bundle path => bundle dependencies
    /// </summary>
    public class BundlesTable : Dictionary<string, BundleDependencySet>
    {
    }

    [Serializable]
    public class BundleDependencySet
    {
        [SerializeField] private List<string> _values = new List<string>();

        public IEnumerable<string> Values => _values;

        public BundleDependencySet(IEnumerable<string> value) {
            _values.AddRange(value);
        }

        public void ForEach(Action<string> action) {
            foreach (var value in _values) action(value);
        }
    }

    [Serializable]
    internal class BundleDependencySetInt
    {
        [SerializeField] private List<int> _values = new List<int>();

        public IEnumerable<int> Values => _values;

        public BundleDependencySetInt(IEnumerable<int> value) {
            _values.AddRange(value);
        }
    }
}