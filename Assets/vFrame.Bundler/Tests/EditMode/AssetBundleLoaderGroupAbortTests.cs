// ------------------------------------------------------------
//         File: AssetBundleLoaderGroupAbortTests.cs
//        Brief: Verifies aborted loader-group teardown releases retained parents and
//               shuts down safely (R12).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:12:01
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using NUnit.Framework;

namespace vFrame.Bundler.Tests.EditMode
{
    /// <summary>
    ///     Guards the R12 abort-path teardown contract: an AssetBundleLoaderGroupSync
    ///     aborted by missing manifest metadata must release the reference it took on
    ///     its parent loader and stay safe to destroy, both explicitly and through
    ///     CollectSystem.
    /// </summary>
    [TestFixture]
    public class AssetBundleLoaderGroupAbortTests
    {
        /// <summary>Asset path deliberately absent from the manifest; drives the missing-asset abort.</summary>
        private const string AssetPath = "Assets/Missing.prefab";
        /// <summary>Bundle name with no manifest entry; mapping an asset to it drives the missing-dependency abort.</summary>
        private const string BundlePath = "missing.bundle";

        /// <summary>
        ///     Log handler that discards every message so expected error logs from
        ///     the abort path never pollute test output.
        /// </summary>
        private sealed class SilentLogHandler : ILogHandler
        {
            /// <summary>Discards the message.</summary>
            public void LogDebug(string text) { }
            /// <summary>Discards the message.</summary>
            public void LogInfo(string text) { }
            /// <summary>Discards the message.</summary>
            public void LogWarning(string text) { }
            /// <summary>Discards the message.</summary>
            public void LogError(string text) { }
            /// <summary>Discards the exception.</summary>
            public void LogException(System.Exception exception) { }
        }

        /// <summary>
        ///     Minimal concrete Loader standing in for an upstream pipeline loader;
        ///     reports completed progress so it adds no scheduling delay.
        /// </summary>
        private sealed class ParentLoader : Loader
        {
            /// <summary>Creates the loader against the shared test contexts.</summary>
            /// <param name="contexts">Contexts wired to the Bundler facade under test.</param>
            public ParentLoader(BundlerContexts contexts)
                : base(contexts, new LoaderContexts()) { }

            /// <summary>Always complete, so the fake parent never blocks scheduling.</summary>
            public override float Progress => 1f;
            /// <summary>Intentional no-op.</summary>
            protected override void OnStart() { }
            /// <summary>Intentional no-op.</summary>
            protected override void OnStop() { }
            /// <summary>Intentional no-op.</summary>
            protected override void OnUpdate() { }
            /// <summary>Intentional no-op.</summary>
            protected override void OnForceComplete() { }
        }

        /// <summary>
        ///     An empty manifest has no entry for the asset path, so the loader group
        ///     aborts; CollectSystem must destroy the aborted group without throwing.
        /// </summary>
        [Test]
        public void CollectionShutdown_MissingAssetMetadata_DoesNotThrow()
        {
            var manifest = new BundlerManifest();

            AssertAbortPathCanBeDestroyedByCollection(manifest);
        }

        /// <summary>
        ///     The asset entry maps to a bundle missing from Bundles, so the loader
        ///     group aborts on dependency lookup; CollectSystem must destroy the
        ///     aborted group without throwing.
        /// </summary>
        [Test]
        public void CollectionShutdown_MissingDependencyMetadata_DoesNotThrow()
        {
            var manifest = new BundlerManifest();
            manifest.Assets.Add(AssetPath, BundlePath);

            AssertAbortPathCanBeDestroyedByCollection(manifest);
        }

        /// <summary>
        ///     Destroying an aborted group must reach Loader.OnDestroy and drop the
        ///     reference taken on its ParentLoader (References back to 0), with the
        ///     teardown sequence staying exception-free.
        /// </summary>
        [Test]
        public void Destroy_MissingAssetMetadata_ReleasesRetainedParent()
        {
            var manifest = new BundlerManifest();
            var options = new BundlerOptions {
                LogHandler = new SilentLogHandler()
            };
            var facade = new Bundler(manifest, options);
            var contexts = new BundlerContexts {
                Bundler = facade,
                Manifest = manifest,
                Options = options
            };

            var parent = new ParentLoader(contexts);
            var loader = new AssetBundleLoaderGroupSync(contexts, new LoaderContexts {
                AssetPath = AssetPath,
                ParentLoader = parent
            });
            var destroyAttempted = false;

            try {
                Assert.That(parent.References, Is.EqualTo(1),
                    "Loader construction must retain its pipeline parent");

                destroyAttempted = true;
                Assert.DoesNotThrow(() => loader.Destroy(),
                    "aborted group teardown must not fail before Loader.OnDestroy");

                Assert.That(parent.References, Is.EqualTo(0),
                    "aborted group teardown must reach Loader.OnDestroy and release its parent");
            }
            finally {
                try {
                    if (!destroyAttempted) {
                        loader.Destroy();
                    }
                }
                finally {
                    try {
                        parent.Destroy();
                    }
                    finally {
                        facade.Destroy();
                    }
                }
            }
        }

        /// <summary>
        ///     Drives the abort path inside a real Bundler facade and asserts the
        ///     aborted group survives CollectSystem destruction without throwing.
        /// </summary>
        /// <param name="manifest">Manifest intentionally missing the metadata the group needs.</param>
        private static void AssertAbortPathCanBeDestroyedByCollection(BundlerManifest manifest)
        {
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