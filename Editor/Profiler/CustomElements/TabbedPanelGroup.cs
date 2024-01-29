// ------------------------------------------------------------
//         File: TabbedPanelGroup.cs
//        Brief: TabbedPanelGroup.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-29 17:46
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class TabbedPanelGroup
    {
        private readonly VisualElement _menuRoot;
        private readonly VisualElement _containerRoot;
        private readonly Dictionary<string, string> _menuToPageNames;
        private readonly string _selectedStyle;
        private Action<string> _callback;

        private TabbedMenu _tabbedMenu;
        private Dictionary<string, VisualElement> _pages;
        private string _selectedTab;
        private string _selectedPage;

        public TabbedPanelGroup(VisualElement menuRoot,
            VisualElement containerRoot,
            Dictionary<string, string> menuToPageNames,
            string selectedStyle = "selected-tab-style") {

            _menuRoot = menuRoot;
            _containerRoot = containerRoot;
            _menuToPageNames = menuToPageNames;
            _selectedStyle = selectedStyle;

            CreateTabbedMenu();
            CreatePages();
        }

        public void SelectTab(string tabName) {
            _tabbedMenu.SelectTab(tabName);
        }

        private void CreateTabbedMenu() {
            _tabbedMenu = new TabbedMenu(_menuRoot, _selectedStyle);
            _tabbedMenu.RegisterCallback(OnSelectedTabChanged);
        }

        private void CreatePages() {
            _pages = new Dictionary<string, VisualElement>();
            foreach (var kv in _menuToPageNames) {
                var panel = _containerRoot.Q<VisualElement>(kv.Value);
                if (null != panel) {
                    _pages.Add(panel.name, panel);
                }
            }
        }

        public void RegisterCallback(Action<string> callback) {
            _callback = callback;
        }

        private void OnSelectedTabChanged(string tabName) {
            _selectedTab = tabName;
            if (!_menuToPageNames.TryGetValue(tabName, out var pageName)) {
                Debug.Log("Cannot find mapping page name for: " + tabName);
                return;
            }

            _selectedPage = pageName;
            UpdatePageVisibility();

            _callback?.Invoke(pageName);
        }

        private void UpdatePageVisibility() {
            foreach (var kv in _pages) {
                kv.Value.style.display = kv.Key == _selectedPage ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public string SelectedPage => _selectedPage;
        public string SelectedTab => _selectedTab;
    }
}

#endif