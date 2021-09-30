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
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Modes;
using vFrame.Bundler.Utils;
using Debug = UnityEngine.Debug;
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
        private List<Node> _children = new List<Node>();
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
            _root = BuildLoaderTree(ref _total, ref _children);

            AsyncRequestHelper.Setup(_context.CoroutinePool, this);
        }

        public IEnumerator OnAsyncProcess() {
            IsStarted = true;

            //var stopWatch = Stopwatch.StartNew();
            _root.Loader.Retain();
            yield return TravelAndLoad();
            _root.Loader.Release();

            //var elapse = stopWatch.Elapsed.TotalSeconds;
            //Debug.LogFormat("LoadRequestAsync finished, cost: {0:0.0000}s, path: {1}", elapse, _root.Loader.AssetBundlePath);

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

        private Node BuildLoaderTree(ref int count, ref List<Node> allChildren) {
            var root = new Node {
                Loader = _bundleLoader
            };
            BuildTree(root, _bundleLoader.Dependencies, ref count, ref allChildren);
            count += 1;
            return root;
        }

        private static void BuildTree(Node parent, IEnumerable<BundleLoaderBase> children, ref int count, ref List<Node> allChildren) {
            foreach (var child in children) {
                var node = new Node {
                    Loader = child
                };
                BuildTree(node, child.Dependencies, ref count, ref allChildren);
                count += 1;

                parent.Children.Add(node);
                allChildren.Add(node);
            }
        }

        private IEnumerator TravelAndLoadNode(Node node, int depth) {
            var stopWatch = Stopwatch.StartNew();
            foreach (var child in node.Children) {
                yield return TravelAndLoadNode(child, depth + 1);
            }

            if (!node.Loader.IsStarted) {
                node.LaunchTime = Time.realtimeSinceStartup;
                node.Loader.Load();
            }

            if (!node.Loader.IsDone) {
                yield return node.Loader;
            }

            var elapse = stopWatch.Elapsed.TotalSeconds;
            //Debug.LogFormat(new string('\t', depth) + "Load loader finished, cost: {0:0.0000}s, path: {1}", elapse, node.Loader.AssetBundlePath);
        }

        private IEnumerator TravelAndLoad() {
            //yield return TravelAndLoadNode(_root, 0);

            //var stopWatch = Stopwatch.StartNew();

            var toLoad = _children.ToList();
            toLoad.Add(_root);

            foreach (var child in toLoad) {
                if (!child.Loader.IsStarted) {
                    child.LaunchTime = Time.realtimeSinceStartup;
                    child.Loader.Load();
                }
            }

            while (true) {
                for (var index = toLoad.Count - 1; index >= 0; index--) {
                    var child = toLoad[index];
                    if (!child.Loader.IsDone) {
                        continue;
                    }

                    //var elapse = stopWatch.Elapsed.TotalSeconds;
                    //Debug.LogFormat("Load loader finished, cost: {0:0.0000}s, path: {1}", elapse, child.Loader.AssetBundlePath);
                    toLoad.RemoveAt(index);
                }

                if (toLoad.Count > 0) {
                    yield return null;
                    continue;
                }
                break;
            }
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