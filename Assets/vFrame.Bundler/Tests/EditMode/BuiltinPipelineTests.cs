// ------------------------------------------------------------
//         File: BuiltinPipelineTests.cs
//        Brief: Verifies empty collections, native failures and successful public pipeline builds.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================

using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using vFrame.Bundler.Editor;

namespace vFrame.Bundler.Tests.EditMode
{
    /// <summary>Regression coverage for formal build failures and successful output.</summary>
    [TestFixture]
    public class BuiltinPipelineTests
    {
        private string _assetPath;
        private string _outputPath;
        private BundleBuildRules _rules;
        private BundleBuildSettings _settings;

        /// <summary>Creates isolated input and output paths without touching project assets.</summary>
        [SetUp]
        public void SetUp()
        {
            var name = "BundlerBuildTests_" + Guid.NewGuid().ToString("N");
            _assetPath = "Assets/" + name;
            _outputPath = "Library/" + name;
            AssetDatabase.CreateFolder("Assets", name);
            _rules = new BundleBuildRules();
            _rules.MainRules.Add(new MainBundleRule {
                PackType = nameof(PackType.PackBySingleFile),
                SearchPath = _assetPath
            });
            _settings = new BundleBuildSettings { BundlePath = _outputPath };
        }

        /// <summary>Removes only this test's assets and generated build output.</summary>
        [TearDown]
        public void TearDown()
        {
            try {
                AssetDatabase.DeleteAsset(_assetPath);
            }
            finally {
                if (Directory.Exists(_outputPath)) {
                    Directory.Delete(_outputPath, true);
                }
            }
        }

        /// <summary>Empty folders must fail before invoking Unity or creating output.</summary>
        /// <param name="dryRun">Whether the caller requests a dry-run build.</param>
        [TestCase(false)]
        [TestCase(true)]
        public void Build_EmptyCollectSet_ReportsRuleAndCreatesNoOutput(bool dryRun)
        {
            _settings.DryRun = dryRun;
            AssertEmptyCollectSet();
            Assert.That(Directory.Exists(_outputPath), Is.False,
                "An empty collection must fail before creating build output.");
        }

        /// <summary>Existing assets excluded by the rules must produce the same clear failure.</summary>
        /// <param name="dryRun">Whether the caller requests a dry-run build.</param>
        [TestCase(false)]
        [TestCase(true)]
        public void Build_FiltersExcludeAllAssets_ReportsRuleAndCreatesNoOutput(bool dryRun)
        {
            CreateTextAsset();
            _rules.MainRules[0].Exclude = ".+";
            _settings.DryRun = dryRun;

            AssertEmptyCollectSet();
            Assert.That(Directory.Exists(_outputPath), Is.False);
        }

        /// <summary>Rejecting an empty collection must not overwrite an earlier build.</summary>
        [Test]
        public void Build_EmptyCollectSet_PreservesExistingOutput()
        {
            Directory.CreateDirectory(_outputPath);
            var manifestPath = Path.Combine(_outputPath, _settings.ManifestFileName);
            const string previousManifest = "previous build manifest";
            File.WriteAllText(manifestPath, previousManifest);

            AssertEmptyCollectSet();

            Assert.That(File.ReadAllText(manifestPath), Is.EqualTo(previousManifest));
            Assert.That(Directory.GetFiles(_outputPath), Has.Length.EqualTo(1));
        }

        /// <summary>An unmatched rule must not prevent another rule from producing a usable bundle.</summary>
        [Test]
        public void Build_NonEmptyCollectSetWithEmptyRule_ProducesLoadableBundle()
        {
            var assetPath = CreateTextAsset();
            _rules.MainRules.Insert(0, new MainBundleRule {
                PackType = nameof(PackType.PackBySingleFile),
                SearchPath = _assetPath,
                Include = "does-not-match"
            });

            BundleGenerator.BuiltinPipeline.Build(_rules, _settings);

            var manifestPath = Path.Combine(_outputPath, _settings.ManifestFileName);
            var manifest = BundlerManifest.FromJson(File.ReadAllText(manifestPath));
            Assert.That(manifest.Assets.Count, Is.EqualTo(1));
            var bundlePath = Path.Combine(_outputPath, manifest.Assets[assetPath]);
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            Assert.That(bundle, Is.Not.Null);
            try {
                Assert.That(bundle.LoadAsset<TextAsset>(assetPath).text, Is.EqualTo("bundle regression"));
            }
            finally {
                bundle.Unload(true);
            }
        }

        /// <summary>A native build failure must stop before outcome validation dereferences its manifest.</summary>
        [Test]
        public void Build_UnityReturnsNoManifest_ReportsBuildContext()
        {
            CreateTextAsset();
            _settings.BundleFormatter = "invalid:bundle.ab";
            LogAssert.Expect(LogType.Error, new Regex("(?i).*invalid:bundle\\.ab.*"));

            var error = Assert.Throws<BundleException>(() =>
                BundleGenerator.BuiltinPipeline.Build(_rules, _settings));

            StringAssert.Contains("Unity returned no manifest", error.Message);
            StringAssert.Contains("1 bundle(s)", error.Message);
            StringAssert.Contains(_settings.BuildTarget.ToString(), error.Message);
            StringAssert.Contains(_outputPath, error.Message);
            Assert.That(File.Exists(Path.Combine(_outputPath, _settings.ManifestFileName)), Is.False);
        }

        /// <summary>Imports a small buildable asset owned by the current test.</summary>
        /// <returns>The imported project-relative asset path.</returns>
        private string CreateTextAsset()
        {
            var assetPath = _assetPath + "/sample.txt";
            File.WriteAllText(assetPath, "bundle regression");
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
            return assetPath;
        }

        /// <summary>Checks the public failure contract for the current empty collection.</summary>
        private void AssertEmptyCollectSet()
        {
            var error = Assert.Throws<BundleException>(() =>
                BundleGenerator.BuiltinPipeline.Build(_rules, _settings));

            StringAssert.Contains("0 bundles", error.Message);
            StringAssert.Contains(_assetPath, error.Message);
            StringAssert.Contains("Include/Exclude", error.Message);
            StringAssert.Contains("asset import errors", error.Message);
        }
    }
}
