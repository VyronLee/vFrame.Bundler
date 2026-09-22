// ------------------------------------------------------------
//         File: ViewBase.cs
//        Brief: UI Toolkit view base: instantiates UXML and binds [ViewElement]-marked fields/properties to elements.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:25:37
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Base class for UI Toolkit views. Instantiates a UXML asset as the view root and binds
    ///     members marked with <see cref="ViewElementAttribute" /> to the matching elements.
    /// </summary>
    internal abstract class ViewBase
    {
        /// <summary>Root element of the instantiated UXML tree; parent of all appended child views.</summary>
        public VisualElement Root { get; }

        /// <summary>Reflection flags for collecting binding candidates; includes inherited non-public members.</summary>
        private const BindingFlags ElementFlags = BindingFlags.Instance
                                                  | BindingFlags.Public
                                                  | BindingFlags.NonPublic
                                                  | BindingFlags.FlattenHierarchy;

        /// <summary>
        ///     Instantiates the UXML asset at <paramref name="uxmlPath" /> as the view root and
        ///     binds all <see cref="ViewElementAttribute" /> marked members.
        /// </summary>
        /// <param name="uxmlPath">Project-relative path of the UXML asset to instantiate.</param>
        protected ViewBase(string uxmlPath)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            Root = visualTree.Instantiate();
            Root.userData = this;
            BindElements();
        }

        /// <summary>
        ///     Appends the root element of another view under this view's root.
        /// </summary>
        /// <typeparam name="T">Concrete view type.</typeparam>
        /// <param name="view">Child view whose <see cref="Root" /> is appended.</param>
        /// <returns>The appended child view, for fluent chaining.</returns>
        public T Add<T>(T view) where T : ViewBase
        {
            Root.Add(view.Root);
            return view;
        }

        /// <summary>Binds all <see cref="ViewElementAttribute" /> marked properties and fields of the derived type.</summary>
        private void BindElements()
        {
            BindPropertiesElement();
            BindFieldsElement();
        }

        /// <summary>Binds all <see cref="ViewElementAttribute" /> marked properties to their queried elements.</summary>
        private void BindPropertiesElement()
        {
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

        /// <summary>Binds all <see cref="ViewElementAttribute" /> marked fields to their queried elements.</summary>
        private void BindFieldsElement()
        {
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

        /// <summary>
        ///     Queries the first element under <see cref="Root" /> matching <paramref name="path" />
        ///     whose type is assignable to <paramref name="elementType" />.
        /// </summary>
        /// <param name="path">Element name or hierarchy path to query.</param>
        /// <param name="elementType">Required element type; must derive from <see cref="VisualElement" />.</param>
        /// <param name="element">Matched element, or null when no match is found.</param>
        /// <returns>True when a matching element was found and written to <paramref name="element" />.</returns>
        private bool QueryElement(string path, Type elementType, out VisualElement element)
        {
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

            bool MatchElementType(VisualElement x)
            {
                return elementType.IsInstanceOfType(x);
            }
        }
    }

    /// <summary>
    ///     Typed view base carrying shared contexts and mutable view data. Assigning a new
    ///     <see cref="ViewData" /> reference triggers <see cref="OnViewDataChanged" />.
    /// </summary>
    /// <typeparam name="T1">Type of the shared contexts object associated with the view.</typeparam>
    /// <typeparam name="T2">Type of the mutable view data rendered by the view.</typeparam>
    internal abstract class ViewBase<T1, T2> : ViewBase where T1 : ViewContexts where T2 : class
    {
        private readonly T1 _contexts;
        private readonly string _uxmlPath;
        private readonly VisualElement _root;

        public T1 Contexts => _contexts;
        public string UxmlPath => _uxmlPath;

        private T2 _viewData;

        /// <summary>
        ///     Instantiates the UXML asset at <paramref name="uxmlPath" /> via the base constructor.
        /// </summary>
        /// <param name="contexts">Shared contexts object supplied by the caller.</param>
        /// <param name="uxmlPath">Project-relative path of the UXML asset to instantiate.</param>
        protected ViewBase(T1 contexts, string uxmlPath) : base(uxmlPath)
        {

        }

        /// <summary>
        ///     View data rendered by this view. The setter raises <see cref="OnViewDataChanged" />
        ///     only when the assigned reference differs from the current one.
        /// </summary>
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

        /// <summary>Called when <see cref="ViewData" /> receives a new reference; subclasses refresh bindings here.</summary>
        protected abstract void OnViewDataChanged();
    }
}

#endif