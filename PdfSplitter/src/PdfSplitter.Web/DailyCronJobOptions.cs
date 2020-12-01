namespace PdfSplitter.Web
{
    public sealed class DailyCronJobOptions
    {
        public const string DailyCronJob = nameof(DailyCronJob);

        public string DirectoryPath { get; set; } = string.Empty;

        public int Hour { get; set; } = 23;

        public int Minute { get; set; }
    }
}
