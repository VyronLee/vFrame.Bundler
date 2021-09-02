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
    public sealed class ResourceAssetAsync : AssetBase, IAssetAsync, IAsyncProcessor
    {
        private ResourceAsyncRequest _request;

        internal ResourceAssetAsync(string assetName, Type type, BundlerContext context)
            : base(assetName, type, null, context) {
        }

        public override void Dispose() {
            if (null != _context && null != _context.CoroutinePool) {
                if (null != _request) {
                    AsyncRequestHelper.Uninstall(_context.CoroutinePool, _request);
                }
                AsyncRequestHelper.Uninstall(_context.CoroutinePool, this);
            }
            base.Dispose();
        }

        public IEnumerator OnAsyncProcess() {
            IsStarted = true;
            yield return _request;

            Logs.Logger.LogInfo("End asynchronously loading asset: {0}", _path);
            IsDone = true;
        }

        public bool IsStarted { get; private set; }

        public float Progress {
            get { return _request == null ? 0f : _request.Progress; }
        }

        public bool MoveNext() {
            return !IsDone;
        }

        public void Reset() {

        }

        public object Current { get; private set; }
        public bool IsSetup { get; set; }
        public int ThreadHandle { get; set; }

        public override Object GetAsset() {
            return null != _request ? _request.asset : null;
        }

        protected override void LoadAssetInternal() {
            Logs.Logger.LogInfo("Start asynchronously loading asset: {0}", _path);

#if UNITY_EDITOR
            if (_context.Options.UseAssetDatabaseInsteadOfResources) {
                _request = new AssetDatabaseAsync(_context);
                _request.LoadAssetAtPath(_path, _type);
            }
#else
            _request = new ResourceAsync();
            _request.LoadAssetAtPath(_path, _type);
#endif

            AsyncRequestHelper.Setup(_context.CoroutinePool, _request);
            AsyncRequestHelper.Setup(_context.CoroutinePool, this);
        }

        private abstract class ResourceAsyncRequest : IAsync, IAsyncProcessor
        {
            public abstract Object asset { get; }
            public void LoadAssetAtPath(string path, Type type) {
                LoadAssetAtPathInternal(path, type);
            }
            protected abstract void LoadAssetAtPathInternal(string path, Type type);
            public abstract bool IsStarted { get; protected set; }
            public bool IsDone { get; protected set; }
            public abstract float Progress { get; }
            public bool MoveNext() {
                return !IsDone;
            }

            public void Reset() {

            }

            public object Current { get; private set; }
            public bool IsSetup { get; set; }
            public int ThreadHandle { get; set; }
            public abstract IEnumerator OnAsyncProcess();
        }

        private class ResourceAsync : ResourceAsyncRequest
        {
            private ResourceRequest _request;

            public override float Progress {
                get {
                    return null == _request ? 0f : _request.progress;
                }
            }

            public override IEnumerator OnAsyncProcess() {
                if (null == _request) {
                    yield break;
                }

                IsStarted = true;
                yield return _request;
                IsDone = true;
            }

            public override Object asset {
                get { return _request.asset; }
            }

            protected override void LoadAssetAtPathInternal(string path, Type type) {
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
            private readonly BundlerContext _context;

            public AssetDatabaseAsync(BundlerContext context) {
                _context = context;
            }

            public override IEnumerator OnAsyncProcess() {
                IsStarted = true;
                while (Time.frameCount - _startFrame < _frameLength) {
                    yield return null;
                }
                IsDone = true;
            }

            public override float Progress {
                get { return Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f); }
            }

            protected override void LoadAssetAtPathInternal(string path, Type type) {
                _path = path;
                _type = type;
                _startFrame = Time.frameCount;
                _frameLength = Random.Range(
                    _context.Options.MinAsyncFrameCountWhenUsingAssetDatabase,
                    _context.Options.MaxAsyncFrameCountWhenUsingAssetDatabase);
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