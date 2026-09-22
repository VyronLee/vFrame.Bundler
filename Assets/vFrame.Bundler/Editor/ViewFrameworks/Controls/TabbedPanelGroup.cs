// ------------------------------------------------------------
//         File: TabbedPanelGroup.cs
//        Brief: Couples a TabbedMenu with a page container: shows only the panel mapped to the selected tab.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:25:33
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Couples a <see cref="TabbedMenu" /> with a page container: selecting a tab shows the panel mapped
    ///     to it and hides the other pages.
    /// </summary>
    internal class TabbedPanelGroup
    {
        private readonly VisualElement _menuRoot;
        private readonly VisualElement _containerRoot;

        /// <summary>Maps tab names to page element names inside the container.</summary>
        private readonly Dictionary<string, string> _menuToPageNames;

        /// <summary>USS class applied by the tab menu to the selected tab.</summary>
        private readonly string _selectedStyle;

        /// <summary>Listener invoked with the mapped page name when the selection changes.</summary>
        private Action<string> _callback;

        /// <summary>Tab strip over <see cref="_menuRoot" />, created at construction.</summary>
        private TabbedMenu _tabbedMenu;

        /// <summary>Page panels by element name, resolved from the container at construction.</summary>
        private Dictionary<string, VisualElement> _pages;

        /// <summary>Currently selected tab name, or null if none.</summary>
        private string _selectedTab;

        /// <summary>Currently visible page name, or null if none.</summary>
        private string _selectedPage;

        /// <summary>
        ///     Build the tab menu over <paramref name="menuRoot" /> and resolve mapped pages under
        ///     <paramref name="containerRoot" />.
        /// </summary>
        /// <param name="menuRoot">Root element hosting the tab menu.</param>
        /// <param name="containerRoot">Root element containing the page panels.</param>
        /// <param name="menuToPageNames">Maps tab names to page element names.</param>
        /// <param name="selectedStyle">USS class applied to the selected tab.</param>
        public TabbedPanelGroup(VisualElement menuRoot,
            VisualElement containerRoot,
            Dictionary<string, string> menuToPageNames,
            string selectedStyle = "selected-tab-style")
        {

            _menuRoot = menuRoot;
            _containerRoot = containerRoot;
            _menuToPageNames = menuToPageNames;
            _selectedStyle = selectedStyle;

            CreateTabbedMenu();
            CreatePages();
        }

        /// <summary>Select the tab with the given name.</summary>
        /// <param name="tabName">Name of the tab to select.</param>
        public void SelectTab(string tabName)
        {
            _tabbedMenu.SelectTab(tabName);
        }

        /// <summary>Create the tab strip over <see cref="_menuRoot" /> and hook its selection callback.</summary>
        private void CreateTabbedMenu()
        {
            _tabbedMenu = new TabbedMenu(_menuRoot, _selectedStyle);
            _tabbedMenu.RegisterCallback(OnSelectedTabChanged);
        }

        /// <summary>
        ///     Resolve all mapped page panels under <see cref="_containerRoot" />; logs an error for a missing panel.
        /// </summary>
        private void CreatePages()
        {
            _pages = new Dictionary<string, VisualElement>();
            foreach (var kv in _menuToPageNames) {
                var panel = _containerRoot.Q<VisualElement>(kv.Value);
                if (null != panel) {
                    _pages.Add(panel.name, panel);
                }
                else {
                    Debug.LogError("Panel not found: " + kv.Value);
                }
            }
        }

        /// <summary>
        ///     Register the listener invoked with the selected tab's page name on selection change.
        /// </summary>
        /// <param name="callback">Listener receiving the page name, or null to clear.</param>
        public void RegisterCallback(Action<string> callback)
        {
            _callback = callback;
        }

        /// <summary>Selection handler: show the mapped page and notify the listener.</summary>
        /// <param name="tabName">Name of the newly selected tab.</param>
        private void OnSelectedTabChanged(string tabName)
        {
            _selectedTab = tabName;
            if (!_menuToPageNames.TryGetValue(tabName, out var pageName)) {
                Debug.Log("Cannot find mapping page name for: " + tabName);
                return;
            }

            _selectedPage = pageName;
            UpdatePageVisibility();

            _callback?.Invoke(pageName);
        }

        /// <summary>Show only the selected page and hide all others.</summary>
        private void UpdatePageVisibility()
        {
            foreach (var kv in _pages) {
                kv.Value.style.display = kv.Key == _selectedPage ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        /// <summary>Name of the currently visible page, or null if none.</summary>
        public string SelectedPage => _selectedPage;

        /// <summary>Name of the currently selected tab, or null if none.</summary>
        public string SelectedTab => _selectedTab;
    }
}

#endif