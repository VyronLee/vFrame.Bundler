using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Profiler
{
    public class BundleProfiler : EditorWindow
    {
        private TextField _clientAddress;
        private ListView _loaders;

        private List<LoaderListItem> _listItems = new List<LoaderListItem>();

        [MenuItem("Tools/vFrame/Bundler/BundleProfiler")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<BundleProfiler>();
            wnd.titleContent = new GUIContent("Bundle Profiler");
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/vFrame.Bundler/Editor/Profiler/BundleProfiler.uxml");
            VisualElement tree = visualTree.Instantiate();

            var root = rootVisualElement;
            root.Add(tree);

            _clientAddress = tree.Q<TextField>("TextFieldClientAddress");

            _loaders = tree.Q<ListView>("ListViewLoaders");
            _loaders.makeItem = () => new LoaderListItem();
            _loaders.bindItem = BindLoaderItem;
            _loaders.unbindItem = UnBindLoaderItem;
            _loaders.itemsSource = Enumerable.Range(0, 100).ToList();
        }

        private void BindLoaderItem(VisualElement element, int index) {
            Debug.Log($"BindLoaderItem, index: {index}");
        }

        private void UnBindLoaderItem(VisualElement element, int index) {
            Debug.Log($"UnBindLoaderItem, index: {index}");
        }

        public void OnDestroy() {

        }
    }
}