// ------------------------------------------------------------
//         File: LoaderListItem.cs
//        Brief: LoaderListItem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-18 18:43
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler
{
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

        public LoaderListItem(ProfilerContexts contexts) :base (contexts, "Pages/Loaders/LoaderListItem.uxml") {

        }

        protected override void OnViewDataChanged() {
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