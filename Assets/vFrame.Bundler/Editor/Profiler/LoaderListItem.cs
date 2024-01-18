// ------------------------------------------------------------
//         File: LoaderListItem.cs
//        Brief: LoaderListItem.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-18 18:43
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEditor;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Profiler
{
    public class LoaderListItem : VisualElement
    {
        public LoaderListItem() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/vFrame.Bundler/Editor/Profiler/LoaderListItem.uxml");
            VisualElement root = visualTree.Instantiate();
            Add(root);
        }
    }
}