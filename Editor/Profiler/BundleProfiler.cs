// ------------------------------------------------------------
//         File: BundleProfiler.cs
//        Brief: BundleProfiler.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-25 21:32
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace vFrame.Bundler
{
    public class BundleProfiler : EditorWindow
    {
        private TextField _clientAddress;
        private Button _buttonStart;
        private Button _buttonClear;
        private ListView _loaders;

        private JsonRpcClient _rpcClient;
        private bool _isStarted;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private const float RefreshFrequency = 2f;

        [MenuItem("Tools/vFrame/Bundler/Profiler")]
        public static void ShowWindow() {
            var wnd = GetWindow<BundleProfiler>();
            wnd.titleContent = new GUIContent("Bundle Profiler");
            wnd.minSize = new Vector2(1280, 720);
        }

        public void CreateGUI() {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/vFrame.Bundler/Editor/Profiler/BundleProfiler.uxml");
            VisualElement tree = visualTree.Instantiate();

            var root = rootVisualElement;
            root.Add(tree);

            _clientAddress = tree.Q<TextField>("TextFieldClientAddress");
            _buttonStart = tree.Q<Button>("ButtonStart");
            _buttonStart.RegisterCallback<ClickEvent>(OnButtonStartClicked);
            _buttonClear = tree.Q<Button>("ButtonClear");
            _buttonClear.RegisterCallback<ClickEvent>(OnButtonClearClicked);

            _loaders = tree.Q<ListView>("ListViewLoaders");
            _loaders.makeItem = () => new LoaderListItem();
            _loaders.bindItem = BindLoaderItem;
            _loaders.unbindItem = UnBindLoaderItem;
        }

        private void OnButtonStartClicked(ClickEvent evt) {
            if (_isStarted) {
                StopProfiler();
            }
            else {
                StartProfiler();
            }
        }

        private void OnButtonClearClicked(ClickEvent evt) {
            _loaders.itemsSource = null;
            _loaders.RefreshItems();
        }

        private void StopProfiler() {
            _isStarted = false;
            _buttonStart.text = "Start";
            _clientAddress.SetEnabled(true);
            _stopwatch.Stop();
        }

        private void StartProfiler() {
            var address = _clientAddress.text;
            if (string.IsNullOrEmpty(address)) {
                Debug.LogWarning("Address is empty.");
                return;
            }
            _rpcClient = JsonRpcClient.CreateSimple(address);
            _isStarted = true;
            _buttonStart.text = "Stop";
            _clientAddress.SetEnabled(false);
            _stopwatch.Restart();
        }

        private void BindLoaderItem(VisualElement element, int index) {
            var listItem = element as LoaderListItem;
            if (null == listItem) {
                return;
            }
            listItem.SetData(_loaders.itemsSource[index] as JsonObject);
        }

        private void UnBindLoaderItem(VisualElement element, int index) {
        }

        public void OnDestroy() {
            StopProfiler();
        }

        private void Update() {
            UpdateRPCClient();
            RequestProfileData();
        }

        private void UpdateRPCClient() {
            if (!_isStarted) {
                return;
            }
            _rpcClient?.Update();
        }

        private void RequestProfileData() {
            if (!_isStarted || IsRefreshmentCooling()) {
                return;
            }
            _stopwatch.Restart();
            _rpcClient.SendRequest(RPCMethods.QueryLoadersInfo, OnQueryLoadersInfoCallback);
        }

        private bool IsRefreshmentCooling() {
            return !_stopwatch.IsRunning || _stopwatch.Elapsed.TotalSeconds < RefreshFrequency;
        }

        private void OnQueryLoadersInfoCallback(JsonObject jsonData) {
            if (!jsonData.TryGetValue("loaders", out var loadersInfo)) {
                return;
            }
            _loaders.itemsSource = loadersInfo as List<object>;
            _loaders.RefreshItems();
        }
    }
}

#endif