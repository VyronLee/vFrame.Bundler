// ------------------------------------------------------------
//         File: AssetHandlerRetainReleaseTests.cs
//        Brief: Regression tests for load-time Retain / Unload-time
//               Release on Asset / AssetAsync handlers (R6).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2026-8-9
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
            public TestLoader() : base(new BundlerContexts(), new LoaderContexts()) { }
            public override float Progress => 1f;
            protected override void OnStart() { }
            protected override void OnStop() { }
            protected override void OnUpdate() { }
            protected override void OnForceComplete() { }
        }

        /// <summary>
        ///     Mirrors LoadSystem.CreateHandler: constructs the handler via the
        ///     constrained generic path (object initializer) so the explicit
        ///     interface Loader setter is invoked without boxing the struct.
        /// </summary>
        private static T Create<T>(Loader loader) where T : ILoaderHandler, new() {
            return new T {
                Loader = loader
            };
        }

        [Test]
        public void Asset_Load_RetainsLoaderOnce() {
            var loader = new TestLoader();
            Assert.That(loader.References, Is.EqualTo(0), "fresh loader has no references");

            Create<Asset>(loader);

            Assert.That(loader.References, Is.EqualTo(1),
                "load must retain the loader once (R6), else Collect reclaims it immediately");
        }

        [Test]
        public void Asset_Unload_ReleasesLoadTimeRetain() {
            var loader = new TestLoader();
            var asset = Create<Asset>(loader);
            Assert.That(loader.References, Is.EqualTo(1));

            asset.Unload();

            Assert.That(loader.References, Is.EqualTo(0),
                "Unload must balance the load-time retain so Collect can reclaim the loader");
        }

        [Test]
        public void Asset_UnloadTwice_OnSameHandle_DoesNotDoubleRelease() {
            var loader = new TestLoader();
            var asset = Create<Asset>(loader);

            asset.Unload();
            asset.Unload();

            Assert.That(loader.References, Is.EqualTo(0),
                "Unload must be idempotent on one handle (IsUnloaded guard)");
        }

        [Test]
        public void AssetAsync_Load_RetainsAndUnload_Releases() {
            var loader = new TestLoader();
            var handler = Create<AssetAsync>(loader);
            Assert.That(loader.References, Is.EqualTo(1),
                "async load must retain the loader once (R6)");

            handler.Unload();
            Assert.That(loader.References, Is.EqualTo(0));
        }

        [Test]
        public void RetainedLoader_IsNotReclaimableBeforeUnload() {
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
