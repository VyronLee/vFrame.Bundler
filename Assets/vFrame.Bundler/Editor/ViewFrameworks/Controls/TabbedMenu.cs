// ------------------------------------------------------------
//         File: TabbedMenu.cs
//        Brief: UI Toolkit tab strip: selects tab buttons under a root element, applies the selected style and
//               notifies listeners.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:27:03
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using System;
using UnityEngine.UIElements;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Tab strip for UI Toolkit: selects tab <see cref="Button" />s under a root element, applies the selected
    ///     style class and notifies a listener with the selected tab name.
    /// </summary>
    internal class TabbedMenu
    {
        /// <summary>USS class added to the currently selected tab button.</summary>
        private readonly string _selectedStyle;

        /// <summary>Root element whose descendant buttons act as tabs.</summary>
        private readonly VisualElement _root;

        /// <summary>Query result over <see cref="_root" />, captured at construction.</summary>
        private UQueryBuilder<Button> _buttons;

        /// <summary>Listener invoked with the tab name when the selection changes.</summary>
        private Action<string> _callback;

        /// <summary>Currently selected tab button, or null if none.</summary>
        private Button _selected;

        /// <summary>
        ///     Create a tab strip over all <see cref="Button" /> descendants of <paramref name="root" />.
        /// </summary>
        /// <param name="root">Root element containing the tab buttons.</param>
        /// <param name="selectedStyle">USS class applied to the selected tab.</param>
        public TabbedMenu(VisualElement root, string selectedStyle = "selected-tab-style")
        {
            _root = root;
            _selectedStyle = selectedStyle;

            FindTabButtons();
        }

        /// <summary>
        ///     Register the listener invoked with the selected tab's element name on selection change.
        /// </summary>
        /// <param name="callback">Listener receiving the tab name, or null to clear.</param>
        public void RegisterCallback(Action<string> callback)
        {
            _callback = callback;
        }

        /// <summary>Capture all descendant tab buttons and hook their click events.</summary>
        private void FindTabButtons()
        {
            _buttons = _root.Query<Button>();
            _buttons.ForEach(button => button.RegisterCallback<ClickEvent>(TabOnClick));
        }

        /// <summary>Click handler that selects the clicked tab button.</summary>
        /// <param name="evt">Click event whose target is selected if it is a <see cref="Button" />.</param>
        private void TabOnClick(ClickEvent evt)
        {
            var button = evt.target as Button;
            if (null == button) {
                return;
            }
            SelectTab(button);
        }

        /// <summary>
        ///     Select a tab button, update styles and notify the listener if the selection changed.
        /// </summary>
        /// <param name="button">Tab button to select.</param>
        /// <param name="dispatch">Whether to invoke the listener on change.</param>
        public void SelectTab(Button button, bool dispatch = true)
        {
            var changed = _selected != button;
            _selected = button;
            UpdateTabStyle();
            if (changed && dispatch) {
                _callback?.Invoke(button.name);
            }
        }

        /// <summary>Select the tab button whose element name equals <paramref name="tabName" />, if any.</summary>
        /// <param name="tabName">Element name of the tab button to select.</param>
        public void SelectTab(string tabName)
        {
            var button = _buttons.Where(btn => btn.name == tabName).First();
            if (null == button) {
                return;
            }
            SelectTab(button);
        }

        /// <summary>Apply the selected style class to the selected tab and remove it from the others.</summary>
        private void UpdateTabStyle()
        {
            _buttons.ForEach(btn => {
                if (_selected == btn) {
                    btn.AddToClassList(_selectedStyle);
                }
                else {
                    btn.RemoveFromClassList(_selectedStyle);
                }
            });
        }
    }
}

#endif