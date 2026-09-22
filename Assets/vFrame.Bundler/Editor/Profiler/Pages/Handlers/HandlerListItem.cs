// ------------------------------------------------------------
//         File: HandlerListItem.cs
//        Brief: Profiler handlers-list row that renders one asset handler's type name, creation frame,
//               asset path and unload state.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:12:49
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// List row for the profiler's Handlers tab; binds one handler JSON row to its labels.
    /// </summary>
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

        /// <summary>
        /// Creates the item and loads its UXML layout from the profiler asset locator directory.
        /// </summary>
        /// <param name="contexts">Shared profiler view contexts.</param>
        public HandlerListItem(ProfilerContexts contexts) : base(contexts, "Pages/Handlers/HandlerListItem.uxml")
        {

        }

        /// <summary>
        /// Refreshes the label texts from the bound handler JSON row
        /// (type name, creation frame, asset path and unload state).
        /// </summary>
        protected override void OnViewDataChanged()
        {
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