// ------------------------------------------------------------
//         File: TabbedMenu.cs
//        Brief: TabbedMenu.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-28 20:46
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using System;
using UnityEngine.UIElements;

namespace vFrame.Bundler
{
    internal class TabbedMenu
    {
        private readonly string _selectedStyle;
        private readonly VisualElement _root;
        private UQueryBuilder<Button> _buttons;
        private Action<string> _callback;
        private Button _selected;

        public TabbedMenu(VisualElement root, string selectedStyle = "selected-tab-style") {
            _root = root;
            _selectedStyle = selectedStyle;

            FindTabButtons();
        }

        public void RegisterCallback(Action<string> callback) {
            _callback = callback;
        }

        private void FindTabButtons() {
            _buttons = _root.Query<Button>();
            _buttons.ForEach(button => button.RegisterCallback<ClickEvent>(TabOnClick));
        }

        private void TabOnClick(ClickEvent evt) {
            var button = evt.target as Button;
            if (null == button) {
                return;
            }
            SelectTab(button);
        }

        public void SelectTab(Button button, bool dispatch = true) {
            var changed = _selected != button;
            _selected = button;
            UpdateTabStyle();
            if (changed && dispatch) {
                _callback?.Invoke(button.name);
            }
        }

        public void SelectTab(string tabName) {
            var button = _buttons.Where(btn => btn.name == tabName).First();
            if (null == button) {
                return;
            }
            SelectTab(button);
        }

        private void UpdateTabStyle() {
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