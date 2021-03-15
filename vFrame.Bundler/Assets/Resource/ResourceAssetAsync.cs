//------------------------------------------------------------
//        File:  ResourceAssetAsync.cs
//       Brief:  Async load assets from resources.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 19:59
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using vFrame.Bundler.Base.Pools;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vFrame.Bundler.Assets.Resource
{
    public sealed class ResourceAssetAsync : AssetBase, IAssetAsync
    {
        private ResourceAsyncRequest _request;

        public ResourceAssetAsync(string assetName, Type type, BundlerOptions options)
            : base(assetName, type, null, options) {
        }

        public IEnumerator Await() {
            if (IsStarted) {
                while (!IsDone) {
                    yield return null;
                }
                yield break;
            }

            IsStarted = true;
            yield return _request.Await();

            Logs.Logger.LogInfo("End asynchronously loading asset: {0}", _path);
            IsDone = true;
        }

        public bool IsStarted { get; private set; }
        public override bool IsDone { get; set; }

        public float Progress {
            get { return _request == null ? 0f : _request.Progress; }
        }

        public override Object GetAsset() {
            return null != _request ? _request.asset : null;
        }

        protected override void LoadAssetInternal() {
            Logs.Logger.LogInfo("Start asynchronously loading asset: {0}", _path);

#if UNITY_EDITOR
            if (_options.UseAssetDatabaseInsteadOfResources) {
                _request = new AssetDatabaseAsync(_options);
                _request.LoadAssetAtPath(_path, _type);
                return;
            }
#endif

            _request = new ResourceAsync();
            _request.LoadAssetAtPath(_path, _type);
        }

        private abstract class ResourceAsyncRequest : IAsync
        {
            public abstract Object asset { get; }
            public abstract void LoadAssetAtPath(string path, Type type);

            public abstract bool IsStarted { get; protected set; }
            public bool IsDone { get; protected set; }
            public abstract float Progress { get; }
            public abstract IEnumerator Await();
        }

        private class ResourceAsync : ResourceAsyncRequest
        {
            private ResourceRequest _request;

            public override float Progress {
                get {
                    return null == _request ? 0f : _request.progress;
                }
            }

            public override IEnumerator Await() {
                if (null == _request) {
                    yield break;
                }

                if (IsStarted) {
                    while (!IsDone) {
                        yield return null;
                    }
                    yield break;
                }

                IsStarted = true;
                yield return _request;
                IsDone = true;
            }

            public override Object asset {
                get { return _request.asset; }
            }

            public override void LoadAssetAtPath(string path, Type type) {
                var resPath = PathUtility.RelativeProjectPathToRelativeResourcesPath(path);

                var sb = StringBuilderPool.Get();
                sb.Append(Path.GetDirectoryName(resPath));
                sb.Append("/");
                sb.Append(Path.GetFileNameWithoutExtension(resPath));

                _request = Resources.LoadAsync(sb.ToString(), type);
                if (_request == null)
                    throw new BundleAssetLoadFailedException(
                        string.Format("Cannot load asset {0} from resources: ", sb));

                StringBuilderPool.Return(sb);
            }

            public override bool IsStarted { get; protected set; }
        }

#if UNITY_EDITOR
        private class AssetDatabaseAsync : ResourceAsyncRequest
        {
            private int _startFrame;
            private int _frameLength;

            private string _path;
            private Type _type;
            private readonly BundlerOptions _options;

            public AssetDatabaseAsync(BundlerOptions options) {
                _options = options;
            }

            public override IEnumerator Await() {
                IsStarted = true;
                while (Time.frameCount - _startFrame < _frameLength) {
                    yield return null;
                }
                IsDone = true;
            }

            public override float Progress {
                get { return Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f); }
            }

            public override void LoadAssetAtPath(string path, Type type) {
                _path = path;
                _type = type;
                _startFrame = Time.frameCount;
                _frameLength = Random.Range(
                    _options.MinAsyncFrameCountWhenUsingAssetDatabase,
                    _options.MaxAsyncFrameCountWhenUsingAssetDatabase);
            }

            public override Object asset {
                get {
                    if (!IsDone) {
                        throw new BundleAssetNotReadyException(
                            string.Format("AssetDatabaseAsync has not finished: {0}", _path));
                    }

                    var obj = AssetDatabase.LoadAssetAtPath(_path, _type);
                    if (!obj)
                        throw new BundleAssetLoadFailedException(
                            string.Format("Cannot load asset {0} from AssetDatabase: ", _path));
                    return obj;
                }
            }

            public override bool IsStarted { get; protected set; }
        }
#endif
    }
}