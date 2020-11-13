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
using System.IO;
using UnityEngine;
using vFrame.Bundler.Exception;
using vFrame.Bundler.Interface;
using vFrame.Bundler.Utils;
using vFrame.Bundler.Utils.Pools;
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
            : base(assetName, type, null, options)
        {
        }

        public bool MoveNext()
        {
            if (_asset)
                return false;

            if (_request == null || _request.keepWaiting)
                return true;

            _asset = _request.asset;

            Logs.Logger.LogInfo("End asynchronously loading asset: {0}", _path);

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
            get { return _request == null ? 0f : _request.progress; }
        }

        public override Object GetAsset()
        {
            return _asset;
        }

        protected override void LoadAssetInternal()
        {
            Logs.Logger.LogInfo("Start asynchronously loading asset: {0}", _path);

#if UNITY_EDITOR
            if (_options.UseAssetDatabaseInsteadOfResources)
            {
                _request = new AssetDatabaseAsync();
                _request.LoadAssetAtPath(_path, _type);
                return;
            }
#endif

            _request = new ResourceAsync();
            _request.LoadAssetAtPath(_path, _type);
        }

        private abstract class ResourceAsyncRequest : CustomYieldInstruction
        {
            public abstract float progress { get; }
            public abstract Object asset { get; }
            public abstract void LoadAssetAtPath(string path, Type type);
        }

        private class ResourceAsync : ResourceAsyncRequest
        {
            private ResourceRequest _request;

            public override bool keepWaiting {
                get { return !_request.isDone; }
            }

            public override float progress {
                get { return _request.progress; }
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
        }

#if UNITY_EDITOR
        private class AssetDatabaseAsync : ResourceAsyncRequest
        {
            private int _startFrame;
            private int _frameLength;

            private string _path;
            private Type _type;

            public override bool keepWaiting {
                get { return Time.frameCount - _startFrame < _frameLength; }
            }

            public override float progress {
                get { return Mathf.Min((float) (Time.frameCount - _startFrame) / _frameLength, 1f); }
            }

            public override void LoadAssetAtPath(string path, Type type) {
                _path = path;
                _type = type;
                _startFrame = Time.frameCount;
                _frameLength = Random.Range(6, 30);
            }

            public override Object asset {
                get {
                    if (keepWaiting) {
                        return null;
                    }

                    var obj = AssetDatabase.LoadAssetAtPath(_path, _type);
                    if (!obj)
                        throw new BundleAssetLoadFailedException(
                            string.Format("Cannot load asset {0} from AssetDatabase: ", _path));
                    return obj;
                }
            }
        }
#endif
    }
}