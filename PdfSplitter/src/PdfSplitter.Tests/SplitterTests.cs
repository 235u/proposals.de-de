using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PdfSplitter.Tests
{
    [TestClass]
    public sealed class SplitterTests
    {
        private static readonly string RelativeDirectoryPath = @"..\..\..\data";
        private static readonly string RelativeInputFilePath = $@"{RelativeDirectoryPath}\123_PB.pdf";
        private static readonly string RelativeOutputFilePath = $@"{RelativeDirectoryPath}\123.pdf";

        private static void OpenWithDefaultReader(string path)
        {
            var processStartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists(RelativeOutputFilePath))
            {
                File.Delete(RelativeOutputFilePath);
            }
        }

        [TestMethod]
        public void TestGetInputFilePaths()
        {
            IEnumerable<string> inputFilePaths = Splitter.GetInputFilePaths(RelativeDirectoryPath);
            string absoluteInputFilePath = new FileInfo(RelativeInputFilePath).FullName;
            Assert.IsTrue(inputFilePaths.Contains(absoluteInputFilePath));
        }

        [TestMethod]
        [Ignore("Validated manually")]
        public void TestSplit()
        {
            string absoluteOutputFilePath = Splitter.Split(RelativeInputFilePath);
            Assert.IsTrue(File.Exists(absoluteOutputFilePath));
            OpenWithDefaultReader(absoluteOutputFilePath);
        }
    }
}
