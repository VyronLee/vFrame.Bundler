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
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class PipelineListItem : ProfilerViewBase<JsonObject>
    {
        [ViewElement]
        private readonly GroupBox _groupbox;

        private readonly ListView _listLoaders;
        private Action<JsonObject> _callback;
        private readonly List<VisualElement> _loaders = new List<VisualElement>();

        public PipelineListItem(ProfilerContexts contexts) : base(contexts, "Pages/Pipelines/PipelineListItem.uxml"){

        }

        protected override void OnViewDataChanged() {
            SetPipelineInfo();
            SetLoaderInfo();
        }

        private void SetPipelineInfo() {
            var isDone = ViewData.SafeGetValue<bool>("IsDone");
            var isError = ViewData.SafeGetValue<bool>("IsError");
            var processing = ViewData.SafeGetValue<bool>("Processing");
            var loaderCount = ViewData.SafeGetValue<int>("LoaderCount");
            var assetPath = ViewData.SafeGetValue<string>("AssetPath");

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

            var loaders = ViewData.SafeGetValue<JsonList>("Loaders");
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