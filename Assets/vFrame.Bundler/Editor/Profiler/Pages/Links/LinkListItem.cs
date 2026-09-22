// ------------------------------------------------------------
//         File: LinkListItem.cs
//        Brief: Profiler list item rendering a link's type name, creation frame, target, and loader's asset path.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:12:53
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Profiler list item that renders one link entry with its type name, creation frame,
    ///     target, and owning loader's asset path.
    /// </summary>
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

        /// <summary>
        ///     Creates the list item view from the <c>Pages/Links/LinkListItem.uxml</c> template.
        /// </summary>
        /// <param name="contexts">Shared profiler contexts consumed by the view base.</param>
        public LinkListItem(ProfilerContexts contexts) : base(contexts, "Pages/Links/LinkListItem.uxml")
        {

        }

        /// <summary>
        ///     Binds the label texts from the current <see cref="ViewData" />, reading the link's
        ///     type name, creation frame, target, and owning loader's asset path.
        /// </summary>
        protected override void OnViewDataChanged()
        {
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