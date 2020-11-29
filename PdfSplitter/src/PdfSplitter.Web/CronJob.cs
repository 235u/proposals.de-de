using Hangfire;

namespace PdfSplitter.Web
{
    public static class CronJob
    {
        public static void Scan(string directoryPath)
        {
            foreach (string inputFilePath in Splitter.GetInputFilePaths(directoryPath))
            {
                BackgroundJob.Enqueue(() => Splitter.Split(inputFilePath));
            }
        }
    }
}
