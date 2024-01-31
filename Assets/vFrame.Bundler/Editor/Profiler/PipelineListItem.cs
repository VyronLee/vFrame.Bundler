// ------------------------------------------------------------
//         File: PipelineListItem.cs
//        Brief: PipelineListItem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 23:19
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class PipelineListItem
    {
        private readonly VisualElement _root;
        private readonly GroupBox _groupbox;
        private readonly ListView _listLoaders;
        private JsonObject _data;
        private Action<JsonObject> _callback;
        private readonly List<VisualElement> _loaders = new List<VisualElement>();

        public PipelineListItem() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/vFrame.Bundler/Editor/Profiler/PipelineListItem.uxml");
            _root = visualTree.Instantiate();
            _root.userData = this;

            _groupbox = _root.Q<GroupBox>();
        }

        public VisualElement Root => _root;

        public void SetData(JsonObject data) {
            if (null == data) {
                return;
            }
            _data = data;

            SetPipelineInfo();
            SetLoaderInfo();
        }

        private void SetPipelineInfo() {
            var isDone = _data.SafeGetValue<bool>("IsDone");
            var isError = _data.SafeGetValue<bool>("IsError");
            var processing = _data.SafeGetValue<bool>("Processing");
            var loaderCount = _data.SafeGetValue<int>("LoaderCount");
            var assetPath = _data.SafeGetValue<string>("AssetPath");

            var sb = new StringBuilder();
            sb.Append("AssetPath: ");
            sb.Append(assetPath);
            sb.Append(", IsDone: ");
            sb.Append(isDone);
            sb.Append(", IsError: ");
            sb.Append(isError);
            sb.Append(", Processing: ");
            sb.Append(processing);
            sb.Append(", LoaderCount: ");
            sb.Append(loaderCount);

            _groupbox.text = sb.ToString();
        }

        private void SetLoaderInfo() {
            _loaders.ForEach(v => v.RemoveFromHierarchy());

            var loaders = _data.SafeGetValue<JsonList>("Loaders");
            foreach (var loader in loaders) {
                var data = loader as JsonObject;
                if (null == data) {
                    continue;
                }
                var typeName = data.SafeGetValue<string>("@TypeName");
                var references = data.SafeGetValue<long>("References");
                var progress = data.SafeGetValue<double>("Progress");
                var elapsed = data.SafeGetValue<double>("Elapsed");
                var taskState = data.SafeGetValue<string>("TaskState");
                var assetPath = data.SafeGetValue<string>("AssetPath");
                var bundlePath = data.SafeGetValue<string>("BundlePath");
                var mainBundlePath = data.SafeGetValue<string>("MainBundlePath");

                var sb = new StringBuilder();
                sb.Append("@TypeName: ");
                sb.Append(typeName);
                sb.Append(", AssetPath: ");
                sb.Append(assetPath ?? bundlePath ?? mainBundlePath);
                sb.Append(", Progress: ");
                sb.Append(progress);
                sb.Append(", Elapsed: ");
                sb.Append(elapsed);
                sb.Append(", TaskState: ");
                sb.Append(taskState);
                sb.Append(", References: ");
                sb.Append(references);

                var label = new Label(sb.ToString());
                label.AddToClassList("pipeline-loader-label");

                _groupbox.Add(label);
                _loaders.Add(label);
            }
        }
    }
}

#endif