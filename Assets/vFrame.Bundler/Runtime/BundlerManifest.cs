// ------------------------------------------------------------
//         File: BundlerManifest.cs
//        Brief: Root manifest mapping asset paths to bundles and bundle dependency sets; JSON round-trip.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:55:07
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Root serializable manifest mapping asset paths to owning bundles and bundle paths to their dependency sets.
    /// Flattened into parallel index-based lists for Unity serialization; restored on deserialization.
    /// </summary>
    [Serializable]
    public class BundlerManifest : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Runtime table: asset path => owning bundle path. Not serialized directly; rebuilt on deserialization.
        /// </summary>
        [NonSerialized] public AssetsTable Assets = new AssetsTable();

        /// <summary>
        /// Runtime table: bundle path => its dependency set. Not serialized directly; rebuilt on deserialization.
        /// </summary>
        [NonSerialized] public BundlesTable Bundles = new BundlesTable();

        /// <summary>
        /// Creates a manifest instance from its JSON representation.
        /// </summary>
        /// <param name="jsonData">JSON data produced from a <see cref="BundlerManifest"/> instance.</param>
        /// <returns>The deserialized manifest instance.</returns>
        /// <exception cref="BundleArgumentException"><paramref name="jsonData"/> is null or empty.</exception>
        public static BundlerManifest FromJson(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new BundleArgumentException($"Argument '{nameof(jsonData)}' cannot be null or empty.");
            return JsonUtility.FromJson<BundlerManifest>(jsonData);
        }

        //=============================================
        // ISerializationCallbackReceiver
        //=============================================

        /// <summary>Sorted asset paths; serialized mirror of <see cref="Assets"/> keys.</summary>
        [SerializeField] private List<string> _assets = new List<string>();

        /// <summary>Index into <see cref="_bundles"/> of the bundle owning each path in <see cref="_assets"/>.</summary>
        [SerializeField] private List<int> _assetContainsInBundle = new List<int>();

        /// <summary>Sorted bundle paths; serialized mirror of <see cref="Bundles"/> keys.</summary>
        [SerializeField] private List<string> _bundles = new List<string>();

        /// <summary>Serialized dependencies of each path in <see cref="_bundles"/>, as indices into <see cref="_bundles"/>.</summary>
        [SerializeField] private List<BundleDependencySetInt> _bundleDependencies = new List<BundleDependencySetInt>();

        /// <summary>
        /// Flattens the runtime tables into parallel, index-based lists for Unity serialization.
        /// </summary>
        public void OnBeforeSerialize()
        {
            _assets = Assets.Keys.ToList();
            _assets.Sort();

            _bundles = Bundles.Keys.ToList();
            _bundles.Sort();

            _assetContainsInBundle = _assets.Select(GetBundleIndex).ToList();
            _bundleDependencies = _bundles.Select(GetDependencySetInt).ToList();
            return;

            int GetBundleIndex(string path)
            {
                return _bundles.IndexOf(Assets[path]);
            }

            BundleDependencySetInt GetDependencySetInt(string path)
            {
                return new BundleDependencySetInt(Bundles[path].Values.Select(v => _bundles.IndexOf(v)));
            }
        }

        /// <summary>
        /// Rebuilds the runtime tables from the serialized parallel lists.
        /// </summary>
        public void OnAfterDeserialize()
        {
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
    /// Table mapping asset paths to the paths of the bundles containing them.
    /// </summary>
    public class AssetsTable : Dictionary<string, string>
    {
    }

    /// <summary>
    /// Table mapping bundle paths to their dependency sets.
    /// </summary>
    public class BundlesTable : Dictionary<string, BundleDependencySet>
    {
    }

    /// <summary>
    /// Immutable set of dependency bundle paths belonging to a single bundle.
    /// </summary>
    [Serializable]
    public class BundleDependencySet
    {
        /// <summary>Dependency bundle paths; Unity-serialized backing store of <see cref="Values"/>.</summary>
        [SerializeField] private List<string> _values = new List<string>();

        /// <summary>Dependency bundle paths contained in this set.</summary>
        public IEnumerable<string> Values => _values;

        /// <summary>
        /// Creates a set containing the given dependency bundle paths.
        /// </summary>
        /// <param name="value">Dependency bundle paths to include.</param>
        public BundleDependencySet(IEnumerable<string> value)
        {
            _values.AddRange(value);
        }

        /// <summary>
        /// Invokes the given action for every dependency bundle path in the set.
        /// </summary>
        /// <param name="action">Action to invoke with each dependency bundle path.</param>
        public void ForEach(Action<string> action)
        {
            foreach (var value in _values) action(value);
        }
    }

    /// <summary>
    /// Serialized form of <see cref="BundleDependencySet"/> using bundle indices instead of paths.
    /// </summary>
    [Serializable]
    internal class BundleDependencySetInt
    {
        /// <summary>Bundle indices; Unity-serialized backing store of <see cref="Values"/>.</summary>
        [SerializeField] private List<int> _values = new List<int>();

        /// <summary>Bundle indices contained in this set.</summary>
        public IEnumerable<int> Values => _values;

        /// <summary>
        /// Creates a set containing the given bundle indices.
        /// </summary>
        /// <param name="value">Bundle indices to include.</param>
        public BundleDependencySetInt(IEnumerable<int> value)
        {
            _values.AddRange(value);
        }
    }
}