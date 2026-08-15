// ------------------------------------------------------------
//         File: HeadlessTestRunner.cs
//        Brief: Self-contained in-process EditMode test runner for
//               the standalone vFrame.Bundler project.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2026-8-9
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace vFrame.Bundler.Editor.TestRunner
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
    ///   -executeMethod vFrame.Bundler.Editor.TestRunner.HeadlessTestRunner.RunAllEditMode `
    ///   -logFile &lt;path&gt;
    ///         </code>
    ///     </para>
    ///     <para>Exit codes: 0 = all pass, 1 = failures, 2 = no tests ran.</para>
    ///     <para>Results: <c>TestResults/headless-editmode-results.xml</c>.</para>
    /// </summary>
    public static class HeadlessTestRunner
    {
        private const string ResultsPath = "TestResults/headless-editmode-results.xml";

        public static void RunAllEditMode() => Run();

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

        private sealed class ResultCollector : ICallbacks
        {
            private readonly List<string> _failures = new List<string>();
            public int PassCount, FailCount, SkipCount, InconclusiveCount;

            public void RunStarted(ITestAdaptor testsToRun) { }

            public void RunFinished(ITestResultAdaptor result)
            {
                PassCount = result.PassCount;
                FailCount = result.FailCount;
                SkipCount = result.SkipCount;
                InconclusiveCount = result.InconclusiveCount;
            }

            public void TestStarted(ITestAdaptor test) { }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.Test.IsSuite) return;
                if (result.TestStatus == TestStatus.Failed) {
                    _failures.Add("[" + result.ResultState + "] " + result.FullName + "\n" +
                                  result.Message + "\n" + result.StackTrace);
                }
            }

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