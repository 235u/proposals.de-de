using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PdfSplitter.Tests
{
    [TestClass]
    public class SplitterTests
    {
        private static readonly string DirectoryPath = @"..\..\..\data";
        private static readonly string InputFilePath = $@"{DirectoryPath}\123_PB.pdf";
        private static readonly string OutputFilePath = $@"{DirectoryPath}\123.pdf";

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
            if (File.Exists(OutputFilePath))
            {
                File.Delete(OutputFilePath);
            }
        }

        [TestMethod]
        public void TestGetInputFilePaths()
        {
            IEnumerable<string> inputFilePaths = Splitter.GetInputFilePaths(DirectoryPath);
            string absolutePath = new FileInfo(InputFilePath).FullName;
            Assert.IsTrue(inputFilePaths.Contains(absolutePath));
        }

        [TestMethod]
        [Ignore("Validated manually")]
        public void TestSplit()
        {
            Splitter.Split(InputFilePath);
            Assert.IsTrue(File.Exists(OutputFilePath));
            OpenWithDefaultReader(OutputFilePath);
        }
    }
}
