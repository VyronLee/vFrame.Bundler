// ------------------------------------------------------------
//         File: AssetBundleLoaderGroupAbortTests.cs
//        Brief: Regression tests for abort-path teardown (R12).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2026-8-10
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================

using NUnit.Framework;

namespace vFrame.Bundler.Tests.EditMode
{
    [TestFixture]
    public class AssetBundleLoaderGroupAbortTests
    {
        private const string AssetPath = "Assets/Missing.prefab";
        private const string BundlePath = "missing.bundle";

        private sealed class SilentLogHandler : ILogHandler
        {
            public void LogDebug(string text) { }
            public void LogInfo(string text) { }
            public void LogWarning(string text) { }
            public void LogError(string text) { }
            public void LogException(System.Exception exception) { }
        }

        [Test]
        public void CollectionShutdown_MissingAssetMetadata_DoesNotThrow() {
            var manifest = new BundlerManifest();

            AssertAbortPathCanBeDestroyedByCollection(manifest);
        }

        [Test]
        public void CollectionShutdown_MissingDependencyMetadata_DoesNotThrow() {
            var manifest = new BundlerManifest();
            manifest.Assets.Add(AssetPath, BundlePath);

            AssertAbortPathCanBeDestroyedByCollection(manifest);
        }

        private static void AssertAbortPathCanBeDestroyedByCollection(BundlerManifest manifest) {
            var options = new BundlerOptions {
                LogHandler = new SilentLogHandler()
            };
            var facade = new Bundler(manifest, options);
            var contexts = new BundlerContexts {
                Bundler = facade,
                Manifest = manifest,
                Options = options
            };

            try {
                var loader = new AssetBundleLoaderGroupSync(contexts, new LoaderContexts {
                    AssetPath = AssetPath
                });
                Assert.That(loader.IsError, Is.True, "incomplete manifest must abort the loader group");
                contexts.AddLoader(loader);

                Assert.DoesNotThrow(() => new CollectSystem(contexts).Destroy());
            }
            finally {
                facade.Destroy();
            }
        }
    }
}
