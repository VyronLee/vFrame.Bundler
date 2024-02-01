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
        [ViewElement("LabelCreateFrame")]
        private readonly Label _labelCreateFrame;

        [ViewElement("LabelPath")]
        private readonly Label _labelPath;

        [ViewElement("LabelIsDone")]
        private readonly Label _labelIsDone;

        [ViewElement("LabelIsError")]
        private readonly Label _labelIsError;

        [ViewElement("LabelProcessing")]
        private readonly Label _labelProcessing;

        [ViewElement("LabelLoaderCount")]
        private readonly Label _labelLoaderCount;

        [ViewElement("FoldoutLoaders")]
        private readonly Foldout _foldoutLoaders;

        [ViewElement("GroupBoxLoaders")]
        private readonly GroupBox _groupBoxLoaders;

        private readonly List<VisualElement> _loaders = new List<VisualElement>();

        private Action<bool> _callback;

        public PipelineListItem(ProfilerContexts contexts) : base(contexts, "Pages/Pipelines/PipelineListItem.uxml"){
            // ReSharper disable once ExpressionIsAlwaysNull
            _foldoutLoaders.RegisterValueChangedCallback(OnFoldoutLoadersValueChanged);
        }

        public void RegisterFoldoutCallback(Action<bool> callback) {
            _callback = callback;
        }

        private void OnFoldoutLoadersValueChanged(ChangeEvent<bool> evt) {
            _callback?.Invoke(evt.newValue);
        }

        protected override void OnViewDataChanged() {
            SetPipelineInfo();
            SetLoaderInfo();
        }

        private void SetPipelineInfo() {
            var createFrame = ViewData.SafeGetValue<int>("CreateFrame");
            var isDone = ViewData.SafeGetValue<bool>("IsDone");
            var isError = ViewData.SafeGetValue<bool>("IsError");
            var processing = ViewData.SafeGetValue<int>("Processing");
            var loaderCount = ViewData.SafeGetValue<int>("LoaderCount");
            var assetPath = ViewData.SafeGetValue<string>("AssetPath");

            _labelCreateFrame.text = createFrame.ToString();
            _labelPath.text = assetPath ?? string.Empty;
            _labelIsDone.text = isDone.ToString();
            _labelIsError.text = isError.ToString();
            _labelProcessing.text = processing.ToString();
            _labelLoaderCount.text = loaderCount.ToString();
        }

        private void SetLoaderInfo() {
            _loaders.ForEach(v => v.RemoveFromHierarchy());

            var loaders = ViewData.SafeGetValue<JsonList>("Loaders");
            for (var i = 0; i < loaders.Count; i++) {
                var loader = loaders[i];
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
                sb.Append($"{i+1}) ");
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

                _groupBoxLoaders.Add(label);
                _loaders.Add(label);
            }
        }
    }
}

#endif