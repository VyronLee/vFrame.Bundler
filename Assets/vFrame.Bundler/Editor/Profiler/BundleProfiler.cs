#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace vFrame.Bundler.Profiler
{
    public class BundleProfiler : EditorWindow
    {
        private TextField _clientAddress;
        private Button _buttonConnect;
        private ListView _loaders;

        private List<LoaderListItem> _listItems = new List<LoaderListItem>();

        private SimpleRPCClient _rpcClient;
        private bool _isConnected = false;
        private readonly Stopwatch _stopwatch = new Stopwatch();

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
            _buttonConnect = tree.Q<Button>("ButtonConnect");
            _buttonConnect.RegisterCallback<ClickEvent>(OnButtonConnectClicked);

            _loaders = tree.Q<ListView>("ListViewLoaders");
            _loaders.makeItem = () => new LoaderListItem();
            _loaders.bindItem = BindLoaderItem;
            _loaders.unbindItem = UnBindLoaderItem;
        }

        private void OnButtonConnectClicked(ClickEvent evt) {
            if (_isConnected) {
                StopProfiler();
            }
            else {
                StartProfiler();
            }
        }

        private void StopProfiler() {
            _isConnected = false;
            _buttonConnect.text = "Connect";
            _stopwatch.Stop();
        }

        private void StartProfiler() {
            var address = _clientAddress.text;
            if (string.IsNullOrEmpty(address)) {
                Debug.LogWarning("Address is empty.");
                return;
            }
            _rpcClient = new SimpleRPCClient(address);
            _isConnected = _rpcClient.Ping();
            if (!_isConnected) {
                return;
            }
            _buttonConnect.text = "Connected";
            _stopwatch.Restart();
        }

        private void BindLoaderItem(VisualElement element, int index) {
            Debug.Log($"BindLoaderItem, index: {index}");
            var listItem = element as LoaderListItem;
            if (null == listItem) {
                return;
            }
            listItem.SetData(_loaders.itemsSource[index] as Dictionary<string, object>);
        }

        private void UnBindLoaderItem(VisualElement element, int index) {
            Debug.Log($"UnBindLoaderItem, index: {index}");
        }

        public void OnDestroy() {
            StopProfiler();
        }

        private void Update() {
            RequestProfileDataIfConnected();
        }

        private void RequestProfileDataIfConnected() {
            if (!_isConnected || _stopwatch.Elapsed.TotalSeconds < 2f) {
                return;
            }
            _stopwatch.Reset();
            _rpcClient.SendRequest(RPCMethods.QueryLoadersInfo, OnQueryLoadersInfoCallback);
        }

        private void OnQueryLoadersInfoCallback(Dictionary<string, object> jsonData) {
            if (jsonData.TryGetValue("loaders", out var loadersInfo)) {
                _loaders.itemsSource = loadersInfo as List<object>;
                _loaders.RefreshItems();
            }
        }
    }
}
#endif