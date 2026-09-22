// ------------------------------------------------------------
//         File: AssetHandlerRetainReleaseTests.cs
//        Brief: EditMode regression tests for the load-time Retain and Unload-time
//               Release reference-count contract on Asset / AssetAsync handlers
//               (audit finding R6).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:12:05
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using NUnit.Framework;

namespace vFrame.Bundler.Tests.EditMode
{
    /// <summary>
    ///     Guards the load-time reference-count contract introduced by audit
    ///     finding R6: loading an asset must hold +1 strong reference on its
    ///     loader (matches Scene, Addressables, YooAsset), balanced by Unload().
    ///     Without it the loader sat at References==0 and CollectSystem reclaimed
    ///     it on the next Collect() — a dangling handle / use-after-free.
    /// </summary>
    [TestFixture]
    public class AssetHandlerRetainReleaseTests
    {
        /// <summary>
        ///     Minimal concrete Loader so the handler's Loader setter (which calls
        ///     Retain on whatever Loader it is handed) can be driven in isolation.
        ///     Avoids Destroy() (which routes through Facade/LogSystem) since these
        ///     tests exercise only the reference-count machinery.
        /// </summary>
        private sealed class TestLoader : Loader
        {
            /// <summary>
            ///     Constructs with fresh contexts; the reference-count path under
            ///     test never exercises them.
            /// </summary>
            public TestLoader() : base(new BundlerContexts(), new LoaderContexts()) { }

            /// <summary>Always reports a completed load so handler logic sees a finished loader.</summary>
            public override float Progress => 1f;

            /// <summary>No-op: no real work to start.</summary>
            protected override void OnStart() { }

            /// <summary>No-op: nothing to stop.</summary>
            protected override void OnStop() { }

            /// <summary>No-op: no per-frame work.</summary>
            protected override void OnUpdate() { }

            /// <summary>No-op: the loader is permanently complete.</summary>
            protected override void OnForceComplete() { }
        }

        /// <summary>
        ///     Mirrors LoadSystem.CreateHandler: constructs the handler via the
        ///     constrained generic path (object initializer) so the explicit
        ///     interface Loader setter is invoked without boxing the struct.
        /// </summary>
        /// <typeparam name="T">Concrete handler type under test (Asset or AssetAsync).</typeparam>
        /// <param name="loader">Loader the handler must retain on construction.</param>
        /// <returns>A new handler holding one load-time retain on <paramref name="loader"/>.</returns>
        private static T Create<T>(Loader loader) where T : ILoaderHandler, new()
        {
            return new T {
                Loader = loader
            };
        }

        /// <summary>Verifies that creating an Asset handler retains its loader exactly once (R6).</summary>
        [Test]
        public void Asset_Load_RetainsLoaderOnce()
        {
            var loader = new TestLoader();
            Assert.That(loader.References, Is.EqualTo(0), "fresh loader has no references");

            Create<Asset>(loader);

            Assert.That(loader.References, Is.EqualTo(1),
                "load must retain the loader once (R6), else Collect reclaims it immediately");
        }

        /// <summary>Verifies that Unload balances the load-time retain, returning the count to zero.</summary>
        [Test]
        public void Asset_Unload_ReleasesLoadTimeRetain()
        {
            var loader = new TestLoader();
            var asset = Create<Asset>(loader);
            Assert.That(loader.References, Is.EqualTo(1));

            asset.Unload();

            Assert.That(loader.References, Is.EqualTo(0),
                "Unload must balance the load-time retain so Collect can reclaim the loader");
        }

        /// <summary>Verifies that Unload is idempotent on one handle and never double-releases.</summary>
        [Test]
        public void Asset_UnloadTwice_OnSameHandle_DoesNotDoubleRelease()
        {
            var loader = new TestLoader();
            var asset = Create<Asset>(loader);

            asset.Unload();
            asset.Unload();

            Assert.That(loader.References, Is.EqualTo(0),
                "Unload must be idempotent on one handle (IsUnloaded guard)");
        }

        /// <summary>Verifies the async handler retains on creation and releases on Unload.</summary>
        [Test]
        public void AssetAsync_Load_RetainsAndUnload_Releases()
        {
            var loader = new TestLoader();
            var handler = Create<AssetAsync>(loader);
            Assert.That(loader.References, Is.EqualTo(1),
                "async load must retain the loader once (R6)");

            handler.Unload();
            Assert.That(loader.References, Is.EqualTo(0));
        }

        /// <summary>Verifies a loaded handle keeps the loader above the CollectSystem reclaim threshold until Unload.</summary>
        [Test]
        public void RetainedLoader_IsNotReclaimableBeforeUnload()
        {
            // CollectSystem.FilterNonReferenceLoader reclaims loaders where
            // IsDone && References <= 0. A loaded-but-not-unloaded handle keeps
            // References > 0, so the loader survives Collect (the R6 invariant).
            var loader = new TestLoader();
            Create<Asset>(loader);

            var wouldReclaim = loader.References <= 0;

            Assert.IsFalse(wouldReclaim,
                "loaded loader must be retained (References > 0) until Unload — "
                + "the R6 use-after-free guard");
        }
    }
}