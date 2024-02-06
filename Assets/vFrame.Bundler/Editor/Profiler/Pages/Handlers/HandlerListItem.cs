// ------------------------------------------------------------
//         File: HandlerListItem.cs
//        Brief: HandlerListItem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-4 19:47
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class HandlerListItem : ProfilerViewBase<JsonObject>
    {
        [ViewElement("LabelCreateFrame")]
        private readonly Label _labelCreateFrame;

        [ViewElement("LabelName")]
        private readonly Label _labelName;

        [ViewElement("LabelPath")]
        private readonly Label _labelPath;

        [ViewElement("LabelIsUnloaded")]
        private readonly Label _labelIsUnloaded;

        public HandlerListItem(ProfilerContexts contexts) : base(contexts, "Pages/Handlers/HandlerListItem.uxml") {

        }

        protected override void OnViewDataChanged() {
            var typeName = ViewData.SafeGetValue<string>("@TypeName");
            var createFrame = ViewData.SafeGetValue<int>("CreateFrame");
            var isUnloaded = ViewData.SafeGetValue<bool>("IsUnloaded");
            var assetPath = ViewData.SafeGetValue<string>("AssetPath");

            _labelCreateFrame.text = createFrame.ToString();
            _labelName.text = typeName ?? string.Empty;
            _labelPath.text = assetPath ?? string.Empty;
            _labelIsUnloaded.text = isUnloaded.ToString();
        }
    }
}

#endif