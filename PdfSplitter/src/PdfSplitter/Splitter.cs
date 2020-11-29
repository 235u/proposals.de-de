using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfSplitter
{
    public static class Splitter
    {
        private const string InputFileNameSuffix = "_PB";
        private const string FileExtension = ".pdf";

        private static readonly string InputFileNameSuffixIncludingExtension = $"{InputFileNameSuffix}{FileExtension}";
        private static readonly string SearchPattern = $"*{InputFileNameSuffixIncludingExtension}";

        static Splitter() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        public static void Split(string inputFilePath)
        {
            using (PdfDocument input = PdfReader.Open(inputFilePath, PdfDocumentOpenMode.Import))
            {
                PdfPage firstPage = input.Pages[0];
                using (var output = new PdfDocument())
                {
                    output.AddPage(firstPage);
                    string outputFilePath = GetOutputFilePath(inputFilePath);
                    output.Save(outputFilePath);
                }
            }
        }

        public static IEnumerable<string> GetInputFilePaths(string directoryPath)
        {
            var inputDirectorInfo = new DirectoryInfo(directoryPath);
            FileInfo[] inputFileInfos = inputDirectorInfo.GetFiles(SearchPattern);
            foreach (FileInfo inputFileInfo in inputFileInfos)
            {
                if (!ActualSplitExists(inputFileInfo))
                {
                    yield return inputFileInfo.FullName;
                }
            }
        }

        private static bool ActualSplitExists(FileInfo inputFileInfo)
        {
            string outputFilePath = GetOutputFilePath(inputFileInfo.FullName);
            if (File.Exists(outputFilePath))
            {
                var outputFileInfo = new FileInfo(outputFilePath);
                return outputFileInfo.LastWriteTime >= inputFileInfo.LastWriteTime;
            }

            return false;
        }

        private static string GetOutputFilePath(string inputFilePath) =>
            inputFilePath.Replace(InputFileNameSuffixIncludingExtension, FileExtension);
    }
}
