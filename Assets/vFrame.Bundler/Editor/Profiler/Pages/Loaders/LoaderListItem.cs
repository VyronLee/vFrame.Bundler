// ------------------------------------------------------------
//         File: LoaderListItem.cs
//        Brief: Profiler list row showing one loader's type, path, ref count, progress, elapsed time and task state.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:12:58
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Profiler list row visualizing a single asset loader: its type, creation frame, best-available path,
    ///     reference count, load progress, elapsed time, and task state.
    /// </summary>
    internal class LoaderListItem : ProfilerViewBase<JsonObject>
    {
        [ViewElement("LabelCreateFrame")]
        private readonly Label _labelCreateFrame;

        [ViewElement("LabelName")]
        private readonly Label _labelName;

        [ViewElement("LabelPath")]
        private readonly Label _labelPath;

        [ViewElement("LabelReferences")]
        private readonly Label _labelRefs;

        [ViewElement("LabelProgress")]
        private readonly Label _labelProgress;

        [ViewElement("LabelElapsed")]
        private readonly Label _labelElapsed;

        [ViewElement("LabelStatus")]
        private readonly Label _labelStatus;

        /// <summary>
        ///     Initializes the row by loading its UXML layout and binding the labels to view elements.
        /// </summary>
        /// <param name="contexts">Shared profiler contexts used by the underlying view.</param>
        public LoaderListItem(ProfilerContexts contexts) : base(contexts, "Pages/Loaders/LoaderListItem.uxml")
        {

        }

        /// <summary>
        ///     Refreshes the row labels when new view data is bound, showing the loader's type, creation frame,
        ///     path (main bundle, then bundle, then asset path, then GUID), progress, elapsed time, task state,
        ///     and reference count.
        /// </summary>
        protected override void OnViewDataChanged()
        {
            var typeName = ViewData.SafeGetValue<string>("@TypeName");
            var createFrame = ViewData.SafeGetValue<int>("CreateFrame");
            var references = ViewData.SafeGetValue<long>("References");
            var progress = ViewData.SafeGetValue<double>("Progress");
            var elapsed = ViewData.SafeGetValue<double>("Elapsed");
            var taskState = ViewData.SafeGetValue<string>("TaskState");
            var assetPath = ViewData.SafeGetValue<string>("AssetPath");
            var bundlePath = ViewData.SafeGetValue<string>("BundlePath");
            var mainBundlePath = ViewData.SafeGetValue<string>("MainBundlePath");
            var guid = ViewData.SafeGetValue<string>("Guid");

            _labelCreateFrame.text = createFrame.ToString();
            _labelName.text = typeName ?? string.Empty;
            _labelPath.text = mainBundlePath ?? bundlePath ?? assetPath ?? guid ?? string.Empty;
            _labelProgress.text = progress.ToString("F2");
            _labelElapsed.text = elapsed.ToString("F2");
            _labelStatus.text = taskState ?? string.Empty;
            _labelRefs.text = references.ToString();
        }
    }
}

#endif