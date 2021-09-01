//------------------------------------------------------------
//        File:  LoadRequestAsync.cs
//       Brief:  LoadRequestAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Modes;
using vFrame.Bundler.Utils;
using Object = UnityEngine.Object;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestAsync : LoadRequest, ILoadRequestAsync, IAsyncProcessor
    {
        private class Node
        {
            public BundleLoaderBase Loader;
            public readonly List<Node> Children = new List<Node>();
            public float LaunchTime;
        }

        private Node _root;
        private int _total;

        internal LoadRequestAsync(ModeBase mode, BundlerContext context, string path, BundleLoaderBase bundleLoader)
            : base(mode, context, path, bundleLoader) {

        }

        public override void Dispose() {
            if (null != _context && null != _context.CoroutinePool) {
                AsyncRequestHelper.Uninstall(_context.CoroutinePool, this);
            }
            base.Dispose();
        }

        protected override void OnLoadProcess() {
            if (null == _bundleLoader) {
                return;
            }
            _root = BuildLoaderTree(ref _total);

            AsyncRequestHelper.Setup(_context.CoroutinePool, this);
        }

        public IEnumerator OnAsyncProcess() {
            IsStarted = true;

            _root.Loader.Retain();
            yield return TravelAndLoad();
            _root.Loader.Release();

            IsDone = true;
        }

        public bool IsStarted { get; private set; }

        public float Progress {
            get {
                var progress = CalculateLoadingProgress();
                return progress / _total;
            }
        }

        public IAssetAsync GetAssetAsync<T>() where T : Object {
            return _mode.GetAssetAsync(this, typeof(T));
        }

        public IAssetAsync GetAssetAsync(Type type) {
            return _mode.GetAssetAsync(this, type);
        }

        public ISceneAsync GetSceneAsync(LoadSceneMode mode) {
            return _mode.GetSceneAsync(this, mode);
        }

        private Node BuildLoaderTree(ref int count) {
            var root = new Node {
                Loader = _bundleLoader
            };
            BuildTree(root, _bundleLoader.Dependencies, ref count);
            count += 1;
            return root;
        }

        private static void BuildTree(Node parent, IEnumerable<BundleLoaderBase> children, ref int count) {
            foreach (var child in children) {
                var node = new Node {
                    Loader = child
                };
                BuildTree(node, child.Dependencies, ref count);
                count += 1;

                parent.Children.Add(node);
            }
        }

        private IEnumerator TravelAndLoadNode(Node node) {
            foreach (var child in node.Children) {
                yield return TravelAndLoadNode(child);
            }

            if (!node.Loader.IsStarted) {
                node.LaunchTime = Time.realtimeSinceStartup;
                node.Loader.Load();
            }

            if (!node.Loader.IsDone) {
                yield return node.Loader;
            }
        }

        private IEnumerator TravelAndLoad() {
            yield return TravelAndLoadNode(_root);
        }

        private float CalculateLoadingProgress() {
            var progress = 0f;
            CalculateNodeProgress(_root, ref progress);
            return progress;
        }

        private void CalculateNodeProgress(Node node, ref float progress) {
            foreach (var child in node.Children)
                CalculateNodeProgress(child, ref progress);

            var loaderAsync = node.Loader as BundleLoaderAsync;
            if (null != loaderAsync)
                progress += loaderAsync.Progress;
            else
                progress += 1;
        }

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current { get; private set; }
        public bool IsSetup { get; set; }
        public int ThreadHandle { get; set; }
    }
}