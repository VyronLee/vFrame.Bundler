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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Profiler
{
    public class LoaderListItem : VisualElement
    {
        private readonly VisualElement _root;
        private Label _labelName;
        private Label _labelPath;
        private Label _labelType;
        private Label _labelProgress;
        private Label _labelStatus;

        public LoaderListItem() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/vFrame.Bundler/Editor/Profiler/LoaderListItem.uxml");
            _root = visualTree.Instantiate();
            Add(_root);

            BindElements();
        }

        private void BindElements() {
            _labelName = _root.Q<Label>("LabelName");
            _labelPath = _root.Q<Label>("LabelPath");
            _labelType = _root.Q<Label>("LabelType");
            _labelProgress = _root.Q<Label>("LabelProgress");
            _labelStatus = _root.Q<Label>("LabelStatus");
        }

        public void SetData(Dictionary<string, object> data) {
            if (null == data) {
                return;
            }

            var typeName = data.SafeGetValue("@TypeName", string.Empty);
            var references = data.SafeGetValue("References", string.Empty);
            var progress = data.SafeGetValue("Progress", string.Empty);
            var taskState = data.SafeGetValue("TaskState", string.Empty);

            _labelName.text = typeName;
            _labelPath.text = "";
            _labelType.text = "";
            _labelProgress.text = progress;
            _labelStatus.text = taskState;
        }


    }
}

#endif