// ------------------------------------------------------------
//         File: OracleRunner.cs
//        Brief: Records public-interface options in an isolated, single-schema Editor session.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-10-18 18:51:08
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Player;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using Debug = UnityEngine.Debug;

namespace vFrame.Bundler.OptionOracle.Editor
{
    /// <summary>Runs one immutable-script phase; the host changes layouts only after this process exits.</summary>
    public sealed class OracleRunner
    {
        private const BuildAssetBundleOptions Common = BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.DisableLoadAssetByFileName |
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
        private readonly Session _session;
        private readonly string _gate;
        private ScriptCompilationResult _scripts;

        /// <summary>Captures the source/schema identity and observations for one process.</summary>
        [Serializable]
        public sealed class Session
        {
            public string phase, track, unityVersion, sbpVersion, sessionId, fatal, sourceBefore, sourceAfter;
            public int processId;
            public string[] editorFields, playerFields;
            public List<Observation> observations = new List<Observation>();
        }

        /// <summary>Preserves raw outcomes; success of the harness does not imply native/SBP equivalence.</summary>
        [Serializable]
        public sealed class Observation
        {
            public string name, backend, options, outputDirectory, returnCode, exception;
            public bool succeeded, loadedValuesValid, useCache, gateRemoved;
            public int errorCount, injectedErrors, serializedWrites = -1, archiveWrites = -1, nativeReportErrors = -1;
            public string[] errors, cacheEntries, bundleNames, hashes, nativeSteps;
        }

        /// <summary>Creates a phase identity without mutable process-wide driver state.</summary>
        private OracleRunner(string phase, string track)
        {
            if (track != "standard" && track != "ignore") throw new ArgumentException("Unknown native cache track.");
            if (phase != "warm" && phase != "layout" && phase != "content") throw new ArgumentException("Unknown phase.");
            _session = new Session {
                phase = phase, track = track, unityVersion = Application.unityVersion, sessionId = Guid.NewGuid().ToString("N"),
                sbpVersion = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(ContentPipeline).Assembly).version
            };
            using (var process = System.Diagnostics.Process.GetCurrentProcess()) {
                _session.processId = process.Id;
            }
            _gate = Environment.GetEnvironmentVariable("VFRAME_ORACLE_INJECT");
            if (string.IsNullOrEmpty(_gate) || File.Exists(_gate)) {
                throw new InvalidOperationException("Missing injection path or stale injection window.");
            }
        }

        /// <summary>Unity executeMethod entry; always writes a phase report before exiting.</summary>
        public static void Run()
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-oraclePhase");
            if (index < 0 || index + 1 == args.Length) throw new ArgumentException("-oraclePhase is required.");
            var trackIndex = Array.IndexOf(args, "-oracleTrack");
            if (trackIndex < 0 || trackIndex + 1 == args.Length) throw new ArgumentException("-oracleTrack is required.");
            var runner = new OracleRunner(args[index + 1], args[trackIndex + 1]);
            var exit = 0;
            try { runner.Execute(); }
            catch (Exception error) { runner._session.fatal = error.ToString(); exit = 3; }
            finally {
                runner._session.sourceAfter = SourceHash();
                if (runner._session.sourceAfter != runner._session.sourceBefore) {
                    runner._session.fatal += "\nRuntime fixture source changed during the Editor session.";
                    exit = 3;
                }
                Directory.CreateDirectory("OracleResults");
                var path = "OracleResults/" + runner._session.phase + ".json";
                File.WriteAllText(path, JsonUtility.ToJson(runner._session, true));
                Debug.Log("ORACLE_PHASE_RESULT=" + Path.GetFullPath(path));
                EditorApplication.Exit(exit);
            }
        }

        /// <summary>Checks the loaded schema and records the selected phase without editing C#.</summary>
        private void Execute()
        {
            if (_session.unityVersion != "2022.3.62f3" || _session.sbpVersion != "1.20.1") {
                throw new InvalidOperationException("This baseline requires Unity 2022.3.62f3 and SBP 1.20.1.");
            }
            if (ScriptableBuildPipeline.UseBuildCacheServer) {
                throw new InvalidOperationException("A remote SBP cache is configured; a local-only cache baseline is required.");
            }
            _session.sourceBefore = SourceHash();
            Directory.CreateDirectory("OracleResults");
            PrepareInputs();
            VerifySchemas();
            if (_session.phase == "content") {
                var root = AssetDatabase.LoadAssetAtPath<OracleAsset>("Assets/OracleInputs/root.asset");
                root.value = 33;
                EditorUtility.SetDirty(root);
                AssetDatabase.SaveAssets();
            }
            // Native option variants need separate project caches, not just different output folders.
            if (_session.track == "ignore") {
                var options = Common | BuildAssetBundleOptions.IgnoreTypeTreeChanges;
                Observe("native", "native-ignore-" + _session.phase, "native-ignore", options);
                if (_session.phase == "warm") Observe("native", "native-ignore-unchanged", "native-ignore", options);
                return;
            }
            if (_session.phase == "warm") {
                Observe("native", "native-warm", "native", Common);
                Observe("native", "native-unchanged", "native", Common);
                Observe("sbp", "sbp-warm", "sbp", Common);
                Observe("sbp", "sbp-unchanged", "sbp", Common);
            }
            else if (_session.phase == "layout") {
                Observe("native", "native-layout", "native", Common);
                Observe("sbp", "sbp-layout", "sbp", Common);
                Observe("sbp", "sbp-layout-unchanged", "sbp", Common);
                Observe("sbp", "sbp-force", "sbp", Common | BuildAssetBundleOptions.ForceRebuildAssetBundle);
                Observe("native", "native-force", "native", Common | BuildAssetBundleOptions.ForceRebuildAssetBundle);
            }
            else if (_session.phase == "content") {
                Observe("native", "native-content", "native", Common);
                Observe("sbp", "sbp-content", "sbp", Common);
                Observe("sbp", "sbp-content-unchanged", "sbp", Common);
                StrictSequence("native");
                StrictSequence("sbp-compat");
            }
            else throw new ArgumentException("Unknown oracle phase: " + _session.phase);
        }

        /// <summary>Creates initial content once and leaves later phases' asset identities unchanged.</summary>
        private void PrepareInputs()
        {
            if (_session.phase != "warm") return;
            Directory.CreateDirectory("Assets/OracleInputs");
            File.WriteAllText("Assets/OracleInputs/note.txt", "oracle-content");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            var leaf = ScriptableObject.CreateInstance<OracleAsset>();
            leaf.value = 22;
            AssetDatabase.CreateAsset(leaf, "Assets/OracleInputs/leaf.asset");
            var root = ScriptableObject.CreateInstance<OracleAsset>();
            root.value = 11;
            root.link = leaf;
            AssetDatabase.CreateAsset(root, "Assets/OracleInputs/root.asset");
            AssetDatabase.SaveAssets();
        }

        /// <summary>Compares real loaded Editor fields with a reflection-only compiled Player type.</summary>
        private void VerifySchemas()
        {
            var settings = new ScriptCompilationSettings {
                group = BuildTargetGroup.Standalone, target = BuildTarget.StandaloneWindows64,
                options = ScriptCompilationOptions.None
            };
            Directory.CreateDirectory("Library/OraclePlayerScripts");
            _scripts = PlayerBuildInterface.CompilePlayerScripts(settings, "Library/OraclePlayerScripts");
            if (_scripts.typeDB == null) throw new InvalidOperationException("No compiled Player TypeDB.");
            _session.editorFields = Fields(typeof(OracleAsset));
            ResolveEventHandler resolve = (sender, argument) => {
                var name = new AssemblyName(argument.Name).Name;
                var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == name);
                return loaded == null ? null : Assembly.ReflectionOnlyLoadFrom(loaded.Location);
            };
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolve;
            try {
                var player = Assembly.ReflectionOnlyLoadFrom(Path.GetFullPath("Library/OraclePlayerScripts/vFrame.Bundler.OptionOracle.dll"));
                _session.playerFields = Fields(player.GetType(typeof(OracleAsset).FullName, true));
            }
            finally { AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolve; }
            var expected = _session.phase == "warm"
                ? new[] { "link:vFrame.Bundler.OptionOracle.OracleAsset", "value:System.Int32" }
                : new[] { "layoutOnly:System.Int32", "link:vFrame.Bundler.OptionOracle.OracleAsset", "value:System.Int32" };
            if (!_session.editorFields.SequenceEqual(expected) || !_session.playerFields.SequenceEqual(expected)) {
                throw new InvalidOperationException("Loaded Editor and compiled Player fields do not match the required schema.");
            }
        }

        /// <summary>Returns stable signatures for this fixture's declared serialized public fields.</summary>
        private static string[] Fields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(f => f.Name + ":" + f.FieldType.FullName).OrderBy(f => f, StringComparer.Ordinal).ToArray();
        }

        /// <summary>Observes raw strict/lenient log errors and invalid inputs with clean recovery runs.</summary>
        private void StrictSequence(string backend)
        {
            var options = Common | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            var prefix = "strict-" + backend + "-";
            Observe(backend, prefix + "clean-before", prefix, options | BuildAssetBundleOptions.StrictMode);
            Observe(backend, prefix + "error-strict", prefix, options | BuildAssetBundleOptions.StrictMode, true);
            Observe(backend, prefix + "clean-after-strict", prefix, options | BuildAssetBundleOptions.StrictMode);
            Observe(backend, prefix + "error-lenient", prefix, options, true);
            Observe(backend, prefix + "clean-after-lenient", prefix, options);
            Observe(backend, prefix + "invalid-strict", prefix, options | BuildAssetBundleOptions.StrictMode, invalidInput: true);
            Observe(backend, prefix + "invalid-lenient", prefix, options, invalidInput: true);
            Observe(backend, prefix + "clean-after-invalid", prefix, options);
        }

        /// <summary>Owns the complete build-window capture and preserves the backend's actual outcome.</summary>
        private void Observe(string backend, string name, string outputKey, BuildAssetBundleOptions options,
            bool inject = false, bool invalidInput = false)
        {
            var result = new Observation {
                backend = backend, name = name, options = options.ToString(),
                outputDirectory = Path.GetFullPath("OracleOutput/" + outputKey),
                useCache = (options & BuildAssetBundleOptions.ForceRebuildAssetBundle) == 0
            };
            Directory.CreateDirectory(result.outputDirectory);
            UnityEngine.Object manifest = null;
            var log = new OracleLog();
            using (var errors = new ErrorCapture()) {
                try {
                    if (inject) File.WriteAllText(_gate, name);
                    var builds = Builds();
                    if (invalidInput) builds[0].assetNames = new[] { "Assets/OracleInputs/missing.asset" };
                    if (backend == "native") {
                        var native = BuildPipeline.BuildAssetBundles(result.outputDirectory, builds, options, BuildTarget.StandaloneWindows64);
                        manifest = native;
                        result.succeeded = native != null;
                        result.returnCode = native == null ? "NullManifest" : "Manifest";
                        if (native != null) {
                            result.bundleNames = native.GetAllAssetBundles().OrderBy(n => n).ToArray();
                            result.hashes = result.bundleNames.Select(n => native.GetAssetBundleHash(n).ToString()).ToArray();
                        }
                        var report = BuildReport.GetLatestReport();
                        result.nativeSteps = report == null ? new string[0] : report.steps.Select(s => s.name).ToArray();
                        result.nativeReportErrors = report == null ? -1 : (int)report.summary.totalErrors;
                    }
                    else if (backend == "sbp-compat") {
                        var compatible = CompatibilityBuildPipeline.BuildAssetBundles(result.outputDirectory, builds, options, BuildTarget.StandaloneWindows64);
                        manifest = compatible;
                        result.succeeded = compatible != null;
                        result.returnCode = compatible == null ? "NullManifest" : "Manifest";
                        if (compatible != null) {
                            result.bundleNames = compatible.GetAllAssetBundles();
                            result.hashes = result.bundleNames.Select(n => compatible.GetAssetBundleHash(n).ToString()).ToArray();
                        }
                    }
                    else {
                        var parameters = new BundleBuildParameters(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, result.outputDirectory) {
                            BundleCompression = UnityEngine.BuildCompression.LZ4, UseCache = result.useCache,
                            ScriptInfo = _scripts.typeDB, ScriptOutputFolder = "Library/OraclePlayerScripts", CacheServerHost = null
                        };
                        IBundleBuildResults output;
                        var code = ContentPipeline.BuildAssetBundles(parameters, new BundleBuildContent(builds), out output,
                            DefaultBuildTasks.Create(DefaultBuildTasks.Preset.AssetBundleCompatible), log);
                        result.returnCode = code.ToString();
                        result.succeeded = code >= ReturnCode.Success && output != null && output.BundleInfos.Count == 3;
                        if (output != null) {
                            result.bundleNames = output.BundleInfos.Keys.OrderBy(n => n).ToArray();
                            result.hashes = result.bundleNames.Select(n => output.BundleInfos[n].Hash.ToString()).ToArray();
                        }
                        result.serializedWrites = log.SerializedWrites;
                        result.archiveWrites = log.ArchiveWrites;
                        result.cacheEntries = log.CacheEntries.ToArray();
                        File.WriteAllText("OracleResults/" + name + "-buildlog.json", log.FormatForTraceEventProfiler());
                    }
                }
                catch (Exception error) {
                    if (string.IsNullOrEmpty(result.returnCode)) result.returnCode = "Exception";
                    result.exception = error.ToString();
                }
                finally {
                    if (inject && File.Exists(_gate)) File.Delete(_gate);
                    if (manifest != null) UnityEngine.Object.DestroyImmediate(manifest);
                    result.gateRemoved = !File.Exists(_gate);
                }
                result.errorCount = errors.Count;
                result.injectedErrors = errors.InjectedCount;
                result.errors = errors.Messages.ToArray();
            }
            if (result.succeeded) {
                try { result.loadedValuesValid = CheckLoadedValues(result.outputDirectory, _session.phase == "content" ? 33 : 11); }
                catch (Exception error) { result.exception += "\nLoading: " + error; }
            }
            _session.observations.Add(result);
            File.WriteAllText("OracleResults/" + _session.phase + ".json", JsonUtility.ToJson(_session, true));
            Debug.Log("ORACLE_OBSERVATION=" + name + " returned=" + result.returnCode + " errors=" + result.errorCount +
                " writes=" + result.serializedWrites + " archives=" + result.archiveWrites);
        }

        /// <summary>Creates three independently observable archives with one explicit object reference.</summary>
        private static AssetBundleBuild[] Builds()
        {
            return new[] {
                new AssetBundleBuild { assetBundleName = "root.ab", assetNames = new[] { "Assets/OracleInputs/root.asset" } },
                new AssetBundleBuild { assetBundleName = "leaf.ab", assetNames = new[] { "Assets/OracleInputs/leaf.asset" } },
                new AssetBundleBuild { assetBundleName = "note.ab", assetNames = new[] { "Assets/OracleInputs/note.txt" } }
            };
        }

        /// <summary>Loads artifacts and checks literal fixture content, releasing every owned bundle.</summary>
        private static bool CheckLoadedValues(string output, int rootValue)
        {
            var loaded = new List<AssetBundle>();
            try {
                foreach (var name in new[] { "leaf.ab", "root.ab", "note.ab" }) {
                    var bundle = AssetBundle.LoadFromFile(Path.Combine(output, name));
                    if (bundle == null) return false;
                    loaded.Add(bundle);
                }
                var root = loaded[1].LoadAsset<OracleAsset>("Assets/OracleInputs/root.asset");
                var note = loaded[2].LoadAsset<TextAsset>("Assets/OracleInputs/note.txt");
                return root != null && root.value == rootValue && root.link != null && root.link.value == 22 &&
                    note != null && note.text == "oracle-content";
            }
            finally { for (var i = loaded.Count - 1; i >= 0; i--) loaded[i].Unload(true); }
        }

        /// <summary>Detects any runtime-script edit within an active Editor process.</summary>
        private static string SourceHash()
        {
            using (var sha = SHA256.Create()) {
                return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes("Assets/Oracle/Runtime/OracleAsset.cs")));
            }
        }

        /// <summary>Captures errors across build threads only for this disposable observation.</summary>
        private sealed class ErrorCapture : IDisposable
        {
            private int _count, _injected;
            public readonly ConcurrentQueue<string> Messages = new ConcurrentQueue<string>();
            public int Count { get { return Volatile.Read(ref _count); } }
            public int InjectedCount { get { return Volatile.Read(ref _injected); } }
            /// <summary>Starts capture before the build call, not at a terminal guard task.</summary>
            public ErrorCapture() { Application.logMessageReceivedThreaded += Capture; }
            /// <summary>Counts every error while retaining a bounded sample and separate stimulus count.</summary>
            private void Capture(string message, string trace, LogType type)
            {
                if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
                var count = Interlocked.Increment(ref _count);
                if (message.Contains("VFRAME_ORACLE_INJECTED:")) Interlocked.Increment(ref _injected);
                if (count <= 12) Messages.Enqueue(message);
            }
            /// <summary>Removes the callback even when the backend throws.</summary>
            public void Dispose() { Application.logMessageReceivedThreaded -= Capture; }
        }

        /// <summary>Observes actual public write/archive events while retaining SBP's own log behavior.</summary>
        private sealed class OracleLog : BuildLog, IBuildLogger
        {
            private int _serialized, _archives;
            public readonly ConcurrentQueue<string> CacheEntries = new ConcurrentQueue<string>();
            public int SerializedWrites { get { return Volatile.Read(ref _serialized); } }
            public int ArchiveWrites { get { return Volatile.Read(ref _archives); } }
            /// <inheritdoc />
            void IBuildLogger.BeginBuildStep(LogLevel level, string stepName, bool threaded)
            {
                if (stepName.StartsWith("Writing ", StringComparison.Ordinal)) Interlocked.Increment(ref _serialized);
                if (stepName == "ArchiveSingleItem") Interlocked.Increment(ref _archives);
                base.BeginBuildStep(level, stepName, threaded);
            }
            /// <inheritdoc />
            void IBuildLogger.AddEntry(LogLevel level, string message)
            {
                if (message.StartsWith("Total Entries:", StringComparison.Ordinal)) CacheEntries.Enqueue(message);
                base.AddEntry(level, message);
            }
            /// <inheritdoc />
            void IBuildLogger.EndBuildStep() { base.EndBuildStep(); }
        }
    }
}