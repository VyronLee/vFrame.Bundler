//------------------------------------------------------------
//        File:  LoadRequestAsync.cs
//       Brief:  LoadRequestAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using UnityEngine;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Modes;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestAsync : LoadRequest, ILoadRequestAsync
    {
        private class Node
        {
            public BundleLoaderBase Loader;
            public readonly List<Node> Children = new List<Node>();
        }

        private Node _root;
        private int _total;

        public LoadRequestAsync(ModeBase mode, string path, BundleLoaderBase bundleLoader)
            : base(mode, path, bundleLoader)
        {

        }

        public bool MoveNext()
        {
            if (_finished)
                return false;

            if (TravelAndLoadLeaf(_root)) {
                return true;
            }

            _finished = true;
            return false;
        }

        public void Reset()
        {
        }

        public object Current { get; private set; }

        public override bool IsDone
        {
            get { return !MoveNext(); }
        }

        public float Progress
        {
            get {
                var progress = CalculateLoadingProgress();
                return progress / _total;
            }
        }

        protected override void LoadInternal() {
            _root = BuildLoaderTree(ref _total);
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
                    Loader = child,
                };
                BuildTree(node, child.Dependencies, ref count);
                count += 1;

                parent.Children.Add(node);
            }
        }

        private bool TravelAndLoadLeaf(Node node) {
            var loading = false;
            foreach (var child in node.Children) {
                var ret = TravelAndLoadLeaf(child);
                loading |= ret;
            }

            if (loading) {
                return true;
            }

            if (!node.Loader.IsDone) {
                if (!node.Loader.IsLoading) {
                    node.Loader.Load();
                }
                return true;
            }
            return false;
        }

        private float CalculateLoadingProgress() {
            var progress = 0f;
            CalculateNodeProgress(_root, ref progress);
            return progress;
        }

        private void CalculateNodeProgress(Node node, ref float progress) {
            foreach (var child in node.Children) {
                CalculateNodeProgress(child, ref progress);
            }

            var loaderAsync = node.Loader as BundleLoaderAsync;
            if (null != loaderAsync) {
                progress += loaderAsync.Progress;
            }
            else {
                progress += 1;
            }
        }

    }
}