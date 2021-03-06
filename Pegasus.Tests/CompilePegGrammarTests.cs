﻿// Copyright © 2015 John Gietzen.  All Rights Reserved.
// This source is subject to the MIT license.
// Please see license.md for more information.

namespace Pegasus.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;
    using NUnit.Framework;

    [TestFixture]
    public class CompilePegGrammarTests
    {
        private static readonly string ErrorGrammar = string.Empty;
        private static readonly string ParseFailGrammar = "start = 'OK";
        private static readonly string PegGrammar = File.ReadAllText("PegParser.peg");
        private static readonly string WarningGrammar = "start = 'OK'; b = 'OK';";

        [Test]
        public void Execute_WhenGivenAGrammarWithErrors_DoesNotCreateTheOutputFile()
        {
            TestTask(ErrorGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(File.Exists(tempCsFile), Is.False);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithErrors_LogsErrors()
        {
            TestTask(ErrorGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(buildEvents.OfType<BuildErrorEventArgs>(), Is.Not.Empty);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithErrors_ReturnsFalse()
        {
            TestTask(ErrorGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(result, Is.False);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithParseErrors_DoesNotCreateTheOutputFile()
        {
            TestTask(ParseFailGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(File.Exists(tempCsFile), Is.False);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithParseErrors_LogsErrors()
        {
            TestTask(ParseFailGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(buildEvents.OfType<BuildErrorEventArgs>(), Is.Not.Empty);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithParseErrors_ReturnsFalse()
        {
            TestTask(ParseFailGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(result, Is.False);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithWarnings_CompilesTheFile()
        {
            TestTask(WarningGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(File.Exists(tempCsFile), Is.True);
                Assert.That(File.ReadAllText(tempCsFile), Is.Not.Empty);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithWarnings_LogsWarnings()
        {
            TestTask(WarningGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(buildEvents.OfType<BuildWarningEventArgs>(), Is.Not.Empty);
            });
        }

        [Test]
        public void Execute_WhenGivenAGrammarWithWarnings_ReturnsTrue()
        {
            TestTask(WarningGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(result, Is.True);
            });
        }

        [Test]
        public void Execute_WhenGivenAnExistingFile_CompilesTheFile()
        {
            TestTask(PegGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(File.Exists(tempCsFile), Is.True);
                Assert.That(File.ReadAllText(tempCsFile), Is.Not.Empty);
            });
        }

        [Test]
        public void Execute_WhenGivenAnExistingFile_DoesNotLogErrors()
        {
            TestTask(PegGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(buildEvents.OfType<BuildErrorEventArgs>(), Is.Empty);
            });
        }

        [Test]
        public void Execute_WhenGivenAnExistingFile_ReturnsTrue()
        {
            TestTask(PegGrammar, (tempPegFile, tempCsFile, result, buildEvents) =>
            {
                Assert.That(result, Is.True);
            });
        }

        private static void TestTask(string pegGrammar, Action<string, string, bool, List<BuildEventArgs>> assert)
        {
            string tempPegFile = null;
            string tempCsFile = null;
            try
            {
                tempPegFile = Path.GetTempFileName();
                File.WriteAllText(tempPegFile, pegGrammar);

                var buildEvents = new List<BuildEventArgs>();
                var task = new CompilePegGrammar
                {
                    InputFile = tempPegFile,
                    BuildEngine = new BuildEngine(buildEvents),
                };

                tempCsFile = tempPegFile + ".cs";
                var result = task.Execute();

                assert(tempPegFile, tempCsFile, result, buildEvents);
            }
            finally
            {
                if (tempPegFile != null)
                {
                    File.Delete(tempPegFile);
                }

                if (tempCsFile != null)
                {
                    File.Delete(tempCsFile);
                }
            }
        }

        private class BuildEngine : IBuildEngine
        {
            private readonly List<BuildEventArgs> buildEvents;

            public BuildEngine(List<BuildEventArgs> buildEvents)
            {
                this.buildEvents = buildEvents;
            }

            public int ColumnNumberOfTaskNode => 0;

            public bool ContinueOnError => false;

            public int LineNumberOfTaskNode => 0;

            public string ProjectFileOfTaskNode => null;

            public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
            {
                throw new NotImplementedException();
            }

            public void LogCustomEvent(CustomBuildEventArgs e) => this.buildEvents.Add(e);

            public void LogErrorEvent(BuildErrorEventArgs e) => this.buildEvents.Add(e);

            public void LogMessageEvent(BuildMessageEventArgs e) => this.buildEvents.Add(e);

            public void LogWarningEvent(BuildWarningEventArgs e) => this.buildEvents.Add(e);
        }
    }
}
