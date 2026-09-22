// ------------------------------------------------------------
//         File: HeadlessTestRunner.cs
//        Brief: Runs EditMode tests in-process via TestRunnerApi under batchmode
//               -executeMethod, writing XML results and the process exit code.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:18:36
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace vFrame.Bundler.TestRunner.Editor
{
    /// <summary>
    ///     In-process EditMode test runner for the standalone Bundler project. Runs tests via
    ///     <c>TestRunnerApi</c> (no Unity Test Protocol port), so it works under
    ///     <c>Unity -batchmode -executeMethod</c> even where <c>-runTests</c> is blocked.
    ///
    ///     <para>
    ///         This is a self-contained copy of vFrame.Core.Unity's HeadlessTestRunner: the Bundler
    ///         is standalone (no vFrame.Core.Unity dependency), so the shared runner cannot be
    ///         referenced. Keep this file in sync with the Core.Unity original.
    ///     </para>
    ///     <para>
    ///         Usage:
    ///         <code>
    /// Unity -batchmode -nographics -quit -projectPath &lt;Bundler&gt; `
    ///   -executeMethod vFrame.Bundler.TestRunner.Editor.HeadlessTestRunner.RunAllEditMode `
    ///   -logFile &lt;path&gt;
    ///         </code>
    ///     </para>
    ///     <para>Exit codes: 0 = all pass, 1 = failures, 2 = no tests ran.</para>
    ///     <para>Results: <c>TestResults/headless-editmode-results.xml</c>.</para>
    /// </summary>
    public static class HeadlessTestRunner
    {
        private const string ResultsPath = "TestResults/headless-editmode-results.xml";

        /// <summary>Parameterless entry point for <c>-executeMethod</c>; runs all EditMode tests.</summary>
        public static void RunAllEditMode() => Run();

        /// <summary>
        ///     Executes EditMode tests synchronously, writes the results XML and exits the editor
        ///     with 0 (all passed), 1 (at least one failure) or 2 (no tests ran).
        /// </summary>
        /// <param name="assemblies">
        ///     Optional test assembly names to restrict the run to; null or empty means all.
        /// </param>
        public static void Run(params string[] assemblies)
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            var collector = new ResultCollector();
            api.RegisterCallbacks(collector);
            api.Execute(new ExecutionSettings(new Filter {
                testMode = TestMode.EditMode,
                assemblyNames = assemblies == null || assemblies.Length == 0 ? null : assemblies
            }) {
                runSynchronously = true
            });
            Finish(collector);
        }

        /// <summary>
        ///     Writes the results XML, logs the pass/fail summary and exits the editor process
        ///     with the appropriate exit code.
        /// </summary>
        /// <param name="collector">Collector holding the finished run's counters and failures.</param>
        private static void Finish(ResultCollector collector)
        {
            Directory.CreateDirectory("TestResults");
            File.WriteAllText(ResultsPath, collector.ToXml(), Encoding.UTF8);
            var total = collector.PassCount + collector.FailCount +
                        collector.SkipCount + collector.InconclusiveCount;
            Debug.Log("[HeadlessTestRunner] passed=" + collector.PassCount +
                      " failed=" + collector.FailCount +
                      " skipped=" + collector.SkipCount +
                      " inconclusive=" + collector.InconclusiveCount +
                      " total=" + total);
            if (total == 0) {
                Debug.LogError("[HeadlessTestRunner] No EditMode tests ran. " +
                               "Check the assembly name(s) and that the test asmdef compiles.");
                EditorApplication.Exit(2);
                return;
            }
            EditorApplication.Exit(collector.FailCount > 0 ? 1 : 0);
        }

        /// <summary>
        ///     Collects run-wide counters and per-test failure details, and serializes them to
        ///     the custom results XML consumed by CI.
        /// </summary>
        private sealed class ResultCollector : ICallbacks
        {
            /// <summary>Descriptions of finished non-suite tests that failed.</summary>
            private readonly List<string> _failures = new List<string>();
            /// <summary>Run-wide result counters copied from the finished run.</summary>
            public int PassCount, FailCount, SkipCount, InconclusiveCount;

            /// <summary>No-op; run start notifications are not used.</summary>
            /// <param name="testsToRun">Root test adaptor for the run about to execute.</param>
            public void RunStarted(ITestAdaptor testsToRun) { }

            /// <summary>Copies the run-wide result counters from the completed run.</summary>
            /// <param name="result">Aggregated result of the completed test run.</param>
            public void RunFinished(ITestResultAdaptor result)
            {
                PassCount = result.PassCount;
                FailCount = result.FailCount;
                SkipCount = result.SkipCount;
                InconclusiveCount = result.InconclusiveCount;
            }

            /// <summary>No-op; per-test start notifications are not used.</summary>
            /// <param name="test">Test adaptor about to start.</param>
            public void TestStarted(ITestAdaptor test) { }

            /// <summary>Records the failure details of a finished non-suite test, if any.</summary>
            /// <param name="result">Result of the finished test.</param>
            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.Test.IsSuite) return;
                if (result.TestStatus == TestStatus.Failed) {
                    _failures.Add("[" + result.ResultState + "] " + result.FullName + "\n" +
                                  result.Message + "\n" + result.StackTrace);
                }
            }

            /// <summary>Serializes the collected counters and failure details to XML.</summary>
            /// <returns>The full text of the custom results XML document.</returns>
            public string ToXml()
            {
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<test-results>");
                sb.AppendLine("  <assembly passed=\"" + PassCount + "\" failed=\"" + FailCount +
                              "\" skipped=\"" + SkipCount + "\" inconclusive=\"" + InconclusiveCount + "\" />");
                foreach (var failure in _failures) {
                    sb.AppendLine("  <failure>");
                    sb.AppendLine("    " + SecurityElement.Escape(failure));
                    sb.AppendLine("  </failure>");
                }
                sb.AppendLine("</test-results>");
                return sb.ToString();
            }
        }
    }
}