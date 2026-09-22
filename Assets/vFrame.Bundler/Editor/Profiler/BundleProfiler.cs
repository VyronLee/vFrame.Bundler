// ------------------------------------------------------------
//         File: BundleProfiler.cs
//        Brief: Editor window that profiles a running Bundler via JSON-RPC: loader, pipeline, handler and link tabs.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:07:50
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Editor window that polls a running Bundler's JSON-RPC profiler endpoint and renders the
    /// loaders, pipelines, handlers and links data in tabbed list views.
    /// </summary>
    public class BundleProfiler : EditorWindow
    {
        private TextField _clientAddress;
        private Button _buttonStart;
        private Button _buttonClear;
        private ListView _loaders;
        private ListView _pipelines;
        private ListView _handlers;
        private ListView _links;
        private TabbedPanelGroup _tabbedPanelGroup;

        /// <summary>JSON-RPC connection to the profiled Bundler; created when polling starts.</summary>
        private JsonRpcClient _rpcClient;
        /// <summary>True while the profiler is polling the remote endpoint.</summary>
        private bool _isStarted;
        /// <summary>Measures the time since the last refresh to throttle requests.</summary>
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly ProfilerLogger _logger = new ProfilerLogger(LogLevel.Debug);
        /// <summary>Shared per-view state passed to the list item views.</summary>
        private readonly ProfilerContexts _contexts = new ProfilerContexts();

        /// <summary>Minimum seconds between consecutive profile data requests.</summary>
        private const float RefreshFrequency = 1f;

        /// <summary>UXML name of the loaders tab button.</summary>
        private const string TabButtonLoadersName = "ButtonLoaders";
        /// <summary>UXML name of the pipelines tab button.</summary>
        private const string TabButtonPipelinesName = "ButtonPipelines";
        /// <summary>UXML name of the handlers tab button.</summary>
        private const string TabButtonHandlersName = "ButtonHandlers";
        /// <summary>UXML name of the links tab button.</summary>
        private const string TabButtonLinksName = "ButtonLinks";

        /// <summary>UXML name of the loaders list page.</summary>
        private const string LoaderListPageName = "LoaderListPage";
        /// <summary>UXML name of the pipelines list page.</summary>
        private const string PipelineListPageName = "PipelineListPage";
        /// <summary>UXML name of the handlers list page.</summary>
        private const string HandlerListPageName = "HandlerListPage";
        /// <summary>UXML name of the links list page.</summary>
        private const string LinkListPageName = "LinkListPage";

        /// <summary>Maps each tab button's UXML name to the UXML name of its list page.</summary>
        private static readonly Dictionary<string, string> _tabNameMapping = new Dictionary<string, string> {
            {TabButtonLoadersName, LoaderListPageName},
            {TabButtonPipelinesName, PipelineListPageName},
            {TabButtonHandlersName, HandlerListPageName},
            {TabButtonLinksName, LinkListPageName}
        };

        /// <summary>Root of the instantiated UXML visual tree.</summary>
        private VisualElement _tree;
        /// <summary>UXML page name of the currently selected tab.</summary>
        private string _selectedPage;

        /// <summary>Menu entry that opens and sizes the profiler window.</summary>
        [MenuItem("Tools/vFrame/Bundler/Profiler")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<BundleProfiler>();
            wnd.titleContent = new GUIContent("Bundle Profiler");
            wnd.minSize = new Vector2(1280, 720);
        }

        /// <summary>Unity callback; instantiates the UXML layout and builds the toolbar, list pages and tab group.</summary>
        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    ProfilerAssetLocator.LocatorDir + "BundleProfiler.uxml");
            _tree = visualTree.Instantiate();
            rootVisualElement.Add(_tree);

            CreateToolbar();
            CreateLoaderListPage();
            CreatePipelineListPage();
            CreateHandlerListPage();
            CreateLinkListPage();
            CreateTabbedPanelGroup();
        }

        /// <summary>Binds the address field and start/clear buttons to their UXML elements and click handlers.</summary>
        private void CreateToolbar()
        {
            _clientAddress = _tree.Q<TextField>("TextFieldClientAddress");
            _buttonStart = _tree.Q<Button>("ButtonStart");
            _buttonStart.RegisterCallback<ClickEvent>(OnButtonStartClicked);
            _buttonClear = _tree.Q<Button>("ButtonClear");
            _buttonClear.RegisterCallback<ClickEvent>(OnButtonClearClicked);
        }

        /// <summary>Builds the tab group that switches between the four list pages.</summary>
        private void CreateTabbedPanelGroup()
        {
            var pageButtons = _tree.Q<VisualElement>("PageButtonGroup");
            var pageContainer = _tree.Q<VisualElement>("PageContainer");
            _tabbedPanelGroup = new TabbedPanelGroup(pageButtons,
                pageContainer,
                _tabNameMapping,
                "tab-button-selected");
            _tabbedPanelGroup.RegisterCallback(OnSelectedPageChanged);
            _tabbedPanelGroup.SelectTab(TabButtonLoadersName);
        }

        /// <summary>Binds the loaders list to loader JSON rows rendered by <see cref="LoaderListItem"/>.</summary>
        private void CreateLoaderListPage()
        {
            _loaders = _tree.Q<ListView>("ListViewLoaders");
            _loaders.makeItem = () => new LoaderListItem(_contexts).Root;
            _loaders.bindItem = (element, index) => {
                var listItem = element.userData as LoaderListItem;
                if (null == listItem) {
                    return;
                }
                listItem.ViewData = _loaders.itemsSource[index] as JsonObject;
            };
        }

        /// <summary>Binds the pipelines list to pipeline JSON rows rendered by <see cref="PipelineListItem"/>.</summary>
        private void CreatePipelineListPage()
        {
            _pipelines = _tree.Q<ListView>("ListViewPipelines");
            _pipelines.makeItem = () => {
                var item = new PipelineListItem(_contexts);
                item.RegisterFoldoutCallback(_ => {
                    _pipelines.RefreshItems();
                });
                return item.Root;
            };
            _pipelines.bindItem = (element, index) => {
                var listItem = element.userData as PipelineListItem;
                if (null == listItem) {
                    return;
                }
                listItem.ViewData = _pipelines.itemsSource[index] as JsonObject;
            };
        }

        /// <summary>Binds the handlers list to handler JSON rows rendered by <see cref="HandlerListItem"/>.</summary>
        private void CreateHandlerListPage()
        {
            _handlers = _tree.Q<ListView>("ListViewHandlers");
            _handlers.makeItem = () => new HandlerListItem(_contexts).Root;
            _handlers.bindItem = (element, index) => {
                var listItem = element.userData as HandlerListItem;
                if (null == listItem) {
                    return;
                }
                listItem.ViewData = _handlers.itemsSource[index] as JsonObject;
            };
        }

        /// <summary>Binds the links list to link JSON rows rendered by <see cref="LinkListItem"/>.</summary>
        private void CreateLinkListPage()
        {
            _links = _tree.Q<ListView>("ListViewLinks");
            _links.makeItem = () => new LinkListItem(_contexts).Root;
            _links.bindItem = (element, index) => {
                var listItem = element.userData as LinkListItem;
                if (null == listItem) {
                    return;
                }
                listItem.ViewData = _links.itemsSource[index] as JsonObject;
            };
        }

        /// <summary>Tab group callback that records the selected page for requests and clearing.</summary>
        /// <param name="pageName">UXML page name of the newly selected tab.</param>
        private void OnSelectedPageChanged(string pageName)
        {
            _selectedPage = pageName;
        }

        /// <summary>Toggles polling on or off depending on the current state.</summary>
        /// <param name="evt">Click event raised by the start/stop button.</param>
        private void OnButtonStartClicked(ClickEvent evt)
        {
            if (_isStarted) {
                StopProfiler();
            }
            else {
                StartProfiler();
            }
        }

        /// <summary>Clears the item source of the currently selected list page.</summary>
        /// <param name="evt">Click event raised by the clear button.</param>
        private void OnButtonClearClicked(ClickEvent evt)
        {
            switch (_selectedPage) {
                case LoaderListPageName:
                    _loaders.itemsSource = null;
                    _loaders.Rebuild();
                    break;
                case PipelineListPageName:
                    _pipelines.itemsSource = null;
                    _pipelines.Rebuild();
                    break;
                case HandlerListPageName:
                    _handlers.itemsSource = null;
                    _handlers.Rebuild();
                    break;
                case LinkListPageName:
                    _links.itemsSource = null;
                    _links.Rebuild();
                    break;
                default:
                    _logger.LogWarning("Unhandled page name: {0}", _selectedPage);
                    break;
            }
        }

        /// <summary>Stops polling and re-enables the address field.</summary>
        private void StopProfiler()
        {
            _isStarted = false;
            _buttonStart.text = "Start";
            _clientAddress.SetEnabled(true);
            _stopwatch.Stop();
        }

        /// <summary>Validates the address, creates the RPC client and starts polling.</summary>
        private void StartProfiler()
        {
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

        /// <summary>Unity callback; stops polling when the window is closed.</summary>
        public void OnDestroy()
        {
            StopProfiler();
        }

        /// <summary>Unity callback; pumps the RPC client and issues periodic refresh requests.</summary>
        private void Update()
        {
            UpdateRPCClient();
            RequestProfileData();
        }

        /// <summary>Pumps the RPC client while polling is active.</summary>
        private void UpdateRPCClient()
        {
            if (!_isStarted) {
                return;
            }
            _rpcClient?.Update();
        }

        /// <summary>Sends the query for the selected page at most once per refresh interval.</summary>
        private void RequestProfileData()
        {
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
                case LinkListPageName:
                    _rpcClient.SendRequest(RPCMethods.QueryLinksInfo, OnQueryLinksInfoCallback);
                    break;
                default:
                    _logger.LogWarning("Unhandled page name: {0}", _selectedPage);
                    break;
            }
        }

        /// <summary>Checks whether another refresh request should be throttled.</summary>
        /// <returns>True while the stopwatch is stopped or the refresh interval has not elapsed.</returns>
        private bool IsRefreshmentCooling()
        {
            return !_stopwatch.IsRunning || _stopwatch.Elapsed.TotalSeconds < RefreshFrequency;
        }

        /// <summary>Applies a loaders query response to its list; silently ignores errors or missing data.</summary>
        /// <param name="respond">Response received from the profiled Bundler.</param>
        private void OnQueryLoadersInfoCallback(RespondContext respond)
        {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }
            var jsonData = respond.RespondData;
            if (!jsonData.TryGetValue("loaders", out var loaders)) {
                return;
            }
            _loaders.itemsSource = loaders as JsonList;
            _loaders.RefreshItems();
        }

        /// <summary>Applies a pipelines query response to its list; silently ignores errors or missing data.</summary>
        /// <param name="respond">Response received from the profiled Bundler.</param>
        private void OnQueryPipelinesInfoCallback(RespondContext respond)
        {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }
            var jsonData = respond.RespondData;
            if (!jsonData.TryGetValue("pipelines", out var pipelines)) {
                return;
            }

            _pipelines.itemsSource = pipelines as JsonList;
            _pipelines.RefreshItems();
        }

        /// <summary>Applies a handlers query response to its list; silently ignores errors or missing data.</summary>
        /// <param name="respond">Response received from the profiled Bundler.</param>
        private void OnQueryHandlersInfoCallback(RespondContext respond)
        {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }

            var jsonData = respond.RespondData;
            if (!jsonData.TryGetValue("handlers", out var handlers)) {
                return;
            }

            _handlers.itemsSource = handlers as JsonList;
            _handlers.RefreshItems();
        }

        /// <summary>Applies a links query response to its list; silently ignores errors or missing data.</summary>
        /// <param name="respond">Response received from the profiled Bundler.</param>
        private void OnQueryLinksInfoCallback(RespondContext respond)
        {
            if (respond.ErrorCode != JsonRpcErrorCode.Success) {
                return;
            }

            var jsonData = respond.RespondData;
            if (!jsonData.TryGetValue("links", out var links)) {
                return;
            }

            _links.itemsSource = links as JsonList;
            _links.RefreshItems();
        }
    }
}

#endif