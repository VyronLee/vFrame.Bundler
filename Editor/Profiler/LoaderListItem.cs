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

using UnityEditor;
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    public class LoaderListItem : VisualElement
    {
        private readonly Label _labelName;
        private readonly Label _labelPath;
        private readonly Label _labelRefs;
        private readonly Label _labelProgress;
        private readonly Label _labelStatus;

        public LoaderListItem() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/vFrame.Bundler/Editor/Profiler/LoaderListItem.uxml");
            var root = visualTree.Instantiate();
            _labelName = root.Q<Label>("LabelName");
            _labelPath = root.Q<Label>("LabelPath");
            _labelProgress = root.Q<Label>("LabelProgress");
            _labelStatus = root.Q<Label>("LabelStatus");
            _labelRefs = root.Q<Label>("LabelReferences");
            Add(root);
        }

        public void SetData(JsonObject data) {
            if (null == data) {
                return;
            }

            var typeName = data.SafeGetValue<string>("@TypeName");
            var references = data.SafeGetValue<string>("References");
            var progress = data.SafeGetValue<string>("Progress");
            var taskState = data.SafeGetValue<string>("TaskState");
            var assetPath = data.SafeGetValue<string>("AssetPath");
            var bundlePath = data.SafeGetValue<string>("BundlePath");
            var mainBundlePath = data.SafeGetValue<string>("MainBundlePath");

            _labelName.text = typeName ?? string.Empty;
            _labelPath.text = mainBundlePath ?? bundlePath ?? assetPath ?? string.Empty;
            _labelProgress.text = progress ?? string.Empty;
            _labelStatus.text = taskState ?? string.Empty;
            _labelRefs.text = references ?? string.Empty;
        }
    }
}

#endif