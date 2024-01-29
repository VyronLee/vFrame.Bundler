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
        private ScrollView _pipelines;
        private TabbedPanelGroup _tabbedPanelGroup;

        private JsonRpcClient _rpcClient;
        private bool _isStarted;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly ProfileLogger _logger = new ProfileLogger();

        private const float RefreshFrequency = 1f;

        private const string TabButtonLoadersName = "ButtonLoaders";
        private const string TabButtonPipelinesName = "ButtonPipelines";
        private const string TabButtonHandlersName = "ButtonHandlers";
        private const string TabButtonLinkedObjectsName = "ButtonLinkedObjects";

        private const string LoaderListPageName = "LoaderListPage";
        private const string PipelineListPageName = "PipelineListPage";
        private const string HandlerListPageName = "HandlerListPage";
        private const string LinkedObjectListPageName = "LinkedObjectListPageName";

        private static readonly Dictionary<string, string> _tabNameMapping = new Dictionary<string, string> {
            {TabButtonLoadersName, LoaderListPageName},
            {TabButtonPipelinesName, PipelineListPageName},
            {TabButtonHandlersName, HandlerListPageName},
            {TabButtonLinkedObjectsName, LinkedObjectListPageName}
        };

        private VisualElement _tree;
        private string _selectedPage;

        [MenuItem("Tools/vFrame/Bundler/Profiler")]
        public static void ShowWindow() {
            var wnd = GetWindow<BundleProfiler>();
            wnd.titleContent = new GUIContent("Bundle Profiler");
            wnd.minSize = new Vector2(1280, 720);
        }

        public void CreateGUI() {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/vFrame.Bundler/Editor/Profiler/BundleProfiler.uxml");
            _tree = visualTree.Instantiate();
            rootVisualElement.Add(_tree);

            CreateToolbar();
            CreateLoaderListPage();
            CreatePipelineListPage();
            CreateTabbedPanelGroup();
        }

        private void CreateToolbar() {
            _clientAddress = _tree.Q<TextField>("TextFieldClientAddress");
            _buttonStart = _tree.Q<Button>("ButtonStart");
            _buttonStart.RegisterCallback<ClickEvent>(OnButtonStartClicked);
            _buttonClear = _tree.Q<Button>("ButtonClear");
            _buttonClear.RegisterCallback<ClickEvent>(OnButtonClearClicked);
        }

        private void CreateTabbedPanelGroup() {
            var pageButtons = _tree.Q<VisualElement>("PageButtonGroup");
            var pageContainer = _tree.Q<VisualElement>("PageContainer");
            _tabbedPanelGroup = new TabbedPanelGroup(pageButtons,
                pageContainer,
                _tabNameMapping,
                "tab-button-selected");
            _tabbedPanelGroup.RegisterCallback(OnSelectedPageChanged);
            _tabbedPanelGroup.SelectTab(TabButtonLoadersName);
        }

        private void CreateLoaderListPage() {
            _loaders = _tree.Q<ListView>("ListViewLoaders");
            _loaders.makeItem = () => new LoaderListItem();
            _loaders.bindItem = BindLoaderItem;
        }

        private void CreatePipelineListPage() {
            _pipelines = _tree.Q<ScrollView>("ScrollViewPipelines");
            _pipelines.contentContainer.Clear();
        }

        private void OnSelectedPageChanged(string pageName) {
            _logger.LogInfo("Selected page changed to: {0}", pageName);
            _selectedPage = pageName;
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
            _rpcClient = JsonRpcClient.CreateSimple(address, _logger);
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

            switch (_selectedPage) {
                case LoaderListPageName:
                    _rpcClient.SendRequest(RPCMethods.QueryLoadersInfo, OnQueryLoadersInfoCallback);
                    break;
                case PipelineListPageName:
                    _rpcClient.SendRequest(RPCMethods.QueryPipelinesInfo, OnQueryPipelinesInfoCallback);
                    break;
                case HandlerListPageName:
                    _rpcClient.SendRequest(RPCMethods.QueryHandlersInfo, OnQueryHandlersInfoCallback);
                    break;
                case LinkedObjectListPageName:
                    _rpcClient.SendRequest(RPCMethods.QueryLinkedObjectsInfo, OnQueryLinkedObjectsInfoCallback);
                    break;
                default:
                    _logger.LogWarning("Unhandled page name: {0}", _selectedPage);
                    break;
            }
        }

        private bool IsRefreshmentCooling() {
            return !_stopwatch.IsRunning || _stopwatch.Elapsed.TotalSeconds < RefreshFrequency;
        }

        private void OnQueryLoadersInfoCallback(RespondContext respond) {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }
            var jsonData = respond.RespondData;
            if (!jsonData.TryGetValue("loaders", out var loadersInfo)) {
                return;
            }
            _loaders.itemsSource = loadersInfo as List<object>;
            _loaders.RefreshItems();
        }

        private void OnQueryPipelinesInfoCallback(RespondContext respond) {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }
            var jsonData = respond.RespondData;
            if (!jsonData.TryGetValue("pipelines", out var pipelinesInfo)) {
                return;
            }

            var pipelines = pipelinesInfo as JsonList;
            if (pipelines == null) {
                return;
            }

            _pipelines.contentContainer.Clear();
            foreach (var pipeline in pipelines) {
                var item = new PipelineListItem();
                item.SetData(pipeline as JsonObject);
                _pipelines.contentContainer.Add(item);
            }
        }

        private void OnQueryHandlersInfoCallback(RespondContext respond) {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }

        }

        private void OnQueryLinkedObjectsInfoCallback(RespondContext respond) {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }

        }
    }
}

#endif