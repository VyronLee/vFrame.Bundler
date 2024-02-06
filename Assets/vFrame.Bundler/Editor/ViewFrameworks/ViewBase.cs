// ------------------------------------------------------------
//         File: ViewBase.cs
//        Brief: ViewBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-1 16:55
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal abstract class ViewBase
    {
        public VisualElement Root { get; }

        private const BindingFlags ElementFlags = BindingFlags.Instance
                                                  | BindingFlags.Public
                                                  | BindingFlags.NonPublic
                                                  | BindingFlags.FlattenHierarchy;

        protected ViewBase(string uxmlPath) {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            Root = visualTree.Instantiate();
            Root.userData = this;
            BindElements();
        }

        public T Add<T>(T view) where T: ViewBase {
            Root.Add(view.Root);
            return view;
        }

        private void BindElements() {
            BindPropertiesElement();
            BindFieldsElement();
        }

        private void BindPropertiesElement() {
            var properties = GetType().GetProperties(ElementFlags);
            foreach (var propertyInfo in properties) {
                var attribute = propertyInfo.GetCustomAttribute<ViewElementAttribute>();
                if (null == attribute) {
                    continue;
                }
                if (QueryElement(attribute.Path, propertyInfo.PropertyType, out var element)) {
                    propertyInfo.SetValue(this, element);
                }
            }
        }

        private void BindFieldsElement() {
            var fields = GetType().GetFields(ElementFlags);
            foreach (var fieldInfo in fields) {
                var attribute = fieldInfo.GetCustomAttribute<ViewElementAttribute>();
                if (null == attribute) {
                    continue;
                }
                if (QueryElement(attribute.Path, fieldInfo.FieldType, out var element)) {
                    fieldInfo.SetValue(this, element);
                }
            }
        }

        private bool QueryElement(string path, Type elementType, out VisualElement element) {
            if (!typeof(VisualElement).IsAssignableFrom(elementType)) {
                Debug.LogError("Invalid view element type: " + elementType);
                element = null;
                return false;
            }
            element = Root.Query(path).Where(MatchElementType).First();
            if (null == element) {
                Debug.LogError("Element not found: " + path + ", type: " + elementType.FullName);
                element = null;
                return false;
            }
            return true;

            bool MatchElementType(VisualElement x) {
                return elementType.IsInstanceOfType(x);
            }
        }
    }

    internal abstract class ViewBase<T1, T2> : ViewBase where T1 : ViewContexts where T2 : class
    {
        private readonly T1 _contexts;
        private readonly string _uxmlPath;
        private readonly VisualElement _root;

        public T1 Contexts => _contexts;
        public string UxmlPath => _uxmlPath;

        private T2 _viewData;

        protected ViewBase(T1 contexts, string uxmlPath) : base(uxmlPath) {

        }

        public T2 ViewData {
            get => _viewData;
            set {
                var changed = _viewData != value;
                _viewData = value;
                if (changed) {
                    OnViewDataChanged();
                }
            }
        }

        protected abstract void OnViewDataChanged();
    }
}

#endif