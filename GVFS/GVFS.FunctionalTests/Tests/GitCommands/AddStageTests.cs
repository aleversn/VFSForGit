﻿using GVFS.FunctionalTests.Category;
using GVFS.FunctionalTests.Tools;
using NUnit.Framework;
using System.Threading;

namespace GVFS.FunctionalTests.Tests.GitCommands
{
    [TestFixture]
    [Category(CategoryConstants.GitCommands)]
    public class AddStageTests : GitRepoTests
    {
        public AddStageTests() : base(enlistmentPerTest: false)
        {
        }

        [TestCase, Order(1)]
        public void AddBasicTest()
        {
            this.EditFile("Readme.md", "Some new content.");
            this.ValidateGitCommand("add Readme.md");
            this.RunGitCommand("commit -m \"Changing the Readme.md\"");
        }

        [TestCase, Order(2)]
        public void StageBasicTest()
        {
            this.EditFile("AuthoringTests.md", "Some new content.");
            this.ValidateGitCommand("stage AuthoringTests.md");
            this.RunGitCommand("commit -m \"Changing the AuthoringTests.md\"");
        }

        [TestCase, Order(3)]
        public void AddAllowsPlaceholderCreation()
        {
            this.CommandAllowsPlaceholderCreation("add", @"GVFS\GVFS\Program.cs");
        }

        [TestCase, Order(4)]
        public void StageAllowsPlaceholderCreation()
        {
            this.CommandAllowsPlaceholderCreation("stage", @"GVFS\GVFS\App.config");
        }

        private void CommandAllowsPlaceholderCreation(string command, string fileToRead)
        {
            this.EditFile("Readme.md", $"Some new content for {command}.");
            ManualResetEventSlim resetEvent = GitHelpers.RunGitCommandWithWaitAndStdIn(this.Enlistment, resetTimeout: 3000, command: $"{command} -p", stdinToQuit: "q");
            this.FileContentsShouldMatch(fileToRead);
            this.ValidateGitCommand("status --no-lock-index");
            resetEvent.Wait();
            this.RunGitCommand("reset --hard");
        }
    }
}
