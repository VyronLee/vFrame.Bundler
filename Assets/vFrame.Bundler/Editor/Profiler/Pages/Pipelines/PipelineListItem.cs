// ------------------------------------------------------------
//         File: PipelineListItem.cs
//        Brief: Profiler list row for one bundle build pipeline; shows pipeline status and a foldout of its loaders'
//               progress and ref counts.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:13:02
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Profiler list row visualizing a single bundler pipeline: status labels plus a foldout
    ///     enumerating the pipeline's loaders with their progress, elapsed time, and reference counts.
    /// </summary>
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

        /// <summary>
        ///     Loader row labels created for the current data; kept so they can be removed when data refreshes.
        /// </summary>
        private readonly List<VisualElement> _loaders = new List<VisualElement>();

        /// <summary>
        ///     Callback invoked when the loaders foldout expands or collapses, receiving the new expanded state.
        /// </summary>
        private Action<bool> _callback;

        /// <summary>
        ///     Initializes the row by loading its UXML layout and wiring the loaders foldout callback.
        /// </summary>
        /// <param name="contexts">Shared profiler contexts used by the underlying view.</param>
        public PipelineListItem(ProfilerContexts contexts) : base(contexts, "Pages/Pipelines/PipelineListItem.uxml")
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            _foldoutLoaders.RegisterValueChangedCallback(OnFoldoutLoadersValueChanged);
        }

        /// <summary>
        ///     Registers a callback invoked whenever the loaders foldout is expanded or collapsed.
        /// </summary>
        /// <param name="callback">Receives the new foldout expanded state.</param>
        public void RegisterFoldoutCallback(Action<bool> callback)
        {
            _callback = callback;
        }

        /// <summary>
        ///     Forwards the foldout's new expanded state to the registered callback.
        /// </summary>
        /// <param name="evt">Foldout change event carrying the expanded state.</param>
        private void OnFoldoutLoadersValueChanged(ChangeEvent<bool> evt)
        {
            _callback?.Invoke(evt.newValue);
        }

        /// <summary>
        ///     Refreshes the row when new view data is bound, updating both status labels and loader rows.
        /// </summary>
        protected override void OnViewDataChanged()
        {
            SetPipelineInfo();
            SetLoaderInfo();
        }

        /// <summary>
        ///     Writes the pipeline's status fields (create frame, asset path, done/error flags,
        ///     processing state, and loader count) into the row labels.
        /// </summary>
        private void SetPipelineInfo()
        {
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

        /// <summary>
        ///     Rebuilds the loader rows from the "Loaders" JSON list, rendering each loader's type,
        ///     path, progress, elapsed time, task state, and reference count as one label.
        /// </summary>
        private void SetLoaderInfo()
        {
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
                sb.Append($"{i + 1}) ");
                sb.Append("@TypeName: ");
                sb.Append(typeName);
                sb.Append(", AssetPath: ");
                sb.Append(assetPath ?? bundlePath ?? mainBundlePath);
                sb.Append(", Progress: ");
                sb.Append(progress);
                sb.Append(", Elapsed(ms): ");
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