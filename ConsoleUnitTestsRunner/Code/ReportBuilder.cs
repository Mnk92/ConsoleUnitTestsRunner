using System.Globalization;
using System.Xml;
using Mnk.Library.Common.Models;
using Mnk.ConsoleUnitTestsRunner.Code.Contracts;
using Mnk.ParallelTests.Contracts;
using NUnit.Framework.Interfaces;

namespace Mnk.ConsoleUnitTestsRunner.Code
{
    internal class ReportBuilder : IReportBuilder
    {
        public void GenerateReport(string path, string xmlReport, ITestsMetricsCalculator tmc, IList<ResultMessage> testResults, double totalTimeInSeconds)
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", "no"));
            var root = doc.AppendChild(CreateElement(doc, "test-results",
                Pair.Create("name", path),
                Pair.Create("total", tmc.Passed.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("errors", tmc.Errors.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("failures", tmc.Failures.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("not-run", tmc.NotRun.Length.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("inconclusive", tmc.Inconclusive.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("ignored", tmc.Ignored.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("skipped", tmc.Skipped.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("invalid", tmc.Invalid.ToString(CultureInfo.InvariantCulture)),
                Pair.Create("date", DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                Pair.Create("time", DateTimeOffset.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture))
                ))!;
            root.AppendChild(CreateElement(doc, "environment",
                Pair.Create("nunit-version", "2.6.3.0"),
                Pair.Create("clr-version", Environment.Version.ToString()),
                Pair.Create("os-version", Environment.OSVersion.ToString()),
                Pair.Create("platform", Environment.OSVersion.Platform.ToString()),
                Pair.Create("cwd", Environment.CurrentDirectory),
                Pair.Create("machine-name", Environment.MachineName),
                Pair.Create("user", Environment.UserName),
                Pair.Create("user-domain", Environment.UserDomainName)
                ));
            root.AppendChild(CreateElement(doc, "culture-info",
                Pair.Create("current-culture", Thread.CurrentThread.CurrentCulture.Name),
                Pair.Create("current-uiculture", Thread.CurrentThread.CurrentUICulture.Name)
                ));

            var testResultsRoot = root;
            if (testResults.Count > 1)
            {
                // Add a summary node, which will be used as root for the test results.
                var executed = testResults.Any(result => result.Executed());
                var anyFailures = tmc.Failed.Any();

                testResultsRoot =
                    root.AppendChild(
                        CreateElement(
                            doc,
                            "test-suite",
                            Pair.Create("type", "Test Project"),
                            Pair.Create("executed", executed.ToString()),
                            Pair.Create(
                                "result",
                                anyFailures ? ResultState.Failure.ToString() : ResultState.Success.ToString()),
                            Pair.Create("success", (!anyFailures).ToString()),
                            Pair.Create("time", string.Format(CultureInfo.InvariantCulture, "{0:0.###}", totalTimeInSeconds)),
                            Pair.Create("asserts", "0")));
            }

            foreach (var r in testResults)
            {
                AppendNode(r, testResultsRoot, doc);
            }

            doc.Save(xmlReport);
        }

        private static void AppendNode(ResultMessage r, XmlNode root, XmlDocument doc)
        {
            var isTest = r.IsTest();
            var el = CreateElement(doc, isTest ? "test-case" : "test-suite",
                    isTest ? null : Pair.Create("type", r.Type),
                    Pair.Create("name", isTest ? r.FullName : r.Key),
                    Pair.Create("executed", r.Executed().ToString()),
                    Pair.Create("result", r.State.ToString()),
                    Pair.Create("success", r.IsSuccess().ToString()),
                    Pair.Create("time", string.Format(CultureInfo.InvariantCulture, "{0:0.###}", r.Duration)),
                    Pair.Create("asserts", r.AssertCount.ToString(CultureInfo.InvariantCulture))
                );
            if (isTest)
            {
                if (r.IsFailed())
                {
                    var f = CreateElement(doc, "failure");
                    if (!string.IsNullOrEmpty(r.Message))
                    {
                        var m = CreateElement(doc, "message");
                        m.AppendChild(doc.CreateCDataSection(r.Message));
                        f.AppendChild(m);
                    }
                    if (!string.IsNullOrEmpty(r.StackTrace))
                    {
                        var m = CreateElement(doc, "stack-trace");
                        m.AppendChild(doc.CreateCDataSection(r.StackTrace));
                        f.AppendChild(m);
                    }
                    el.AppendChild(f);
                }
            }
            else
            {
                var results = CreateElement(doc, "results");
                el.AppendChild(results);
                foreach (var test in r.Children)
                {
                    AppendNode(test, results, doc);
                }
            }
            root.AppendChild(el);
        }

        private static XmlElement CreateElement(XmlDocument doc, string name, params Pair<string, string>[] attributes)
        {
            var el = doc.CreateElement(name);
            foreach (var a in attributes.Where(x => x != null))
            {
                var atr = doc.CreateAttribute(a.Key);
                atr.Value = a.Value;
                el.Attributes.Append(atr);
            }
            return el;
        }
    }
}
