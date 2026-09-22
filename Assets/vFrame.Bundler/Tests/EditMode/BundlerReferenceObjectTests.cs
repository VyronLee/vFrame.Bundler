// ------------------------------------------------------------
//         File: BundlerReferenceObjectTests.cs
//        Brief: Regression tests for the reference-count Destroy-cascade underflow guard (R5).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:12:09
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using System;
using NUnit.Framework;

namespace vFrame.Bundler.Tests.EditMode
{
    /// <summary>
    /// Guards the reference-count machinery on BundlerReferenceObject against the
    /// Destroy-cascade underflow described in audit finding R5.
    ///
    /// Cascade scenario: CollectSystem.DestroyAllLoaders() iterates loaders and
    /// calls loader.Destroy() in dictionary order. When a parent loader is
    /// destroyed before a child, BundlerReferenceObject.Destroy() zeroes the
    /// parent's _references; the child's Loader.OnDestroy() later calls
    /// ReleaseParent() -> parent.Release(), which previously underflowed and
    /// threw a false InvalidOperationException during normal shutdown.
    /// </summary>
    [TestFixture]
    public class BundlerReferenceObjectTests
    {
        /// <summary>
        /// Minimal concrete subclass so the internal reference-count API can be
        /// driven in isolation. The Retain/Release/Destroy machinery never touches
        /// BundlerContexts, so a default instance is sufficient.
        /// </summary>
        private sealed class RefObject : BundlerReferenceObject
        {
            public RefObject() : base(new BundlerContexts()) { }
            protected override void OnDestroy() { }
        }

        /// <summary>
        /// A release on an already-destroyed object (child loader releasing a
        /// parent destroyed first in the cascade) must neither throw nor drive
        /// the reference count negative.
        /// </summary>
        [Test]
        public void Release_AfterDestroy_DoesNotThrowAndDoesNotUnderflow()
        {
            // Simulates a child loader releasing an already-destroyed parent
            // (child.Loader.OnDestroy -> ReleaseParent -> parent.Release).
            var parent = new RefObject();
            parent.Retain();            // child's RetainParent()
            Assert.That(parent.References, Is.EqualTo(1));

            parent.Destroy();           // parent destroyed first in the cascade
            Assert.That(parent.References, Is.EqualTo(0));

            Assert.DoesNotThrow(() => parent.Release());
            Assert.That(parent.References, Is.EqualTo(0),
                "release on a destroyed object must not drive the count negative");
        }

        /// <summary>
        /// Retaining an already-destroyed object must be a graceful no-op,
        /// not resurrect the reference count.
        /// </summary>
        [Test]
        public void Retain_AfterDestroy_IsGracefulNoOp()
        {
            var obj = new RefObject();
            obj.Retain();
            obj.Destroy();

            Assert.DoesNotThrow(() => obj.Retain());
            Assert.That(obj.References, Is.EqualTo(0),
                "retain on a destroyed object must be a no-op, not increment");
        }

        /// <summary>
        /// Legitimate underflow detection on live (non-destroyed) objects
        /// must remain intact so real leaks are still caught.
        /// </summary>
        [Test]
        public void Release_WithoutRetain_OnLiveObject_StillThrows()
        {
            // Regression guard: legitimate underflow detection for live
            // (non-destroyed) objects must remain intact to catch real leaks.
            var obj = new RefObject();
            Assert.Throws<InvalidOperationException>(() => obj.Release());
        }
    }
}