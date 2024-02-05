// ------------------------------------------------------------
//         File: LinkListItem.cs
//        Brief: LinkListItem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-5 17:57
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class LinkListItem : ProfilerViewBase<JsonObject>
    {
        [ViewElement("LabelCreateFrame")]
        private readonly Label _labelCreateFrame;

        [ViewElement("LabelName")]
        private readonly Label _labelName;

        [ViewElement("LabelTarget")]
        private readonly Label _labelTarget;

        [ViewElement("LabelLoader")]
        private readonly Label _labelLoader;

        public LinkListItem(ProfilerContexts contexts) : base(contexts, "Pages/Links/LinkListItem.uxml") {

        }

        protected override void OnViewDataChanged() {
            var typeName = ViewData.SafeGetValue<string>("@TypeName");
            var createFrame = ViewData.SafeGetValue<int>("CreateFrame");
            var target = ViewData.SafeGetValue<string>("vFrame.Bundler.ILink.Target");
            var loader = ViewData.SafeGetValue<JsonObject>("vFrame.Bundler.ILink.Loader");
            var assetPath = loader?.SafeGetValue<string>("AssetPath");

            _labelCreateFrame.text = createFrame.ToString();
            _labelName.text = typeName ?? string.Empty;
            _labelTarget.text = target ?? string.Empty;
            _labelLoader.text = assetPath ?? string.Empty;
        }
    }
}

#endif