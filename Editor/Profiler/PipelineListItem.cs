// ------------------------------------------------------------
//         File: PipelineListItem.cs
//        Brief: PipelineListItem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 23:19
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEditor;
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class PipelineListItem : VisualElement
    {
        private readonly Foldout _foldout;
        private JsonObject _data;

        public PipelineListItem() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/vFrame.Bundler/Editor/Profiler/PipelineListItem.uxml");
            var root = visualTree.Instantiate();
            _foldout = root.Q<Foldout>("FoldoutInfo");
            _foldout.RegisterValueChangedCallback(OnFoldoutValueChanged);
            Add(root);
        }

        public void SetData(JsonObject data) {
            if (null == data) {
                return;
            }
            _data = data;

            var guid = data.SafeGetValue<string>("Guid");
            var isDone = data.SafeGetValue<bool>("IsDone");
            var isError = data.SafeGetValue<bool>("IsError");
            var processing = data.SafeGetValue<bool>("Processing");
            var loaderCount = data.SafeGetValue<int>("LoaderCount");
            var assetPath = data.SafeGetValue<string>("AssetPath");

            var info = $"Guid: {guid}, AssetPath: {assetPath}, IsDone: {isDone},"
                       + $" IsError: {isError}, Processing: {processing}, LoaderCount: {loaderCount}";
            _foldout.text = info;
        }

        private void OnFoldoutValueChanged(ChangeEvent<bool> evt) {

        }
    }
}