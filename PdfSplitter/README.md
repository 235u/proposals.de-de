# Automatisches PDF-Splitting

S. https://www.twago.de/project/automatisches-pdf-splitting/163969/ (bzw. [Druckversion](src/PdfSplitter.Tests/data/123_PB.pdf))

## Umsetzung

`appsettings.json`

```json
{
  "DailyCronJob": {
    "Hour": 23,
    "Minute": 0,
    "DirectoryPath": "..\\PdfSplitter.Tests\\data"
  }
}
```

![Recurring jobs](docs/recurring-jobs.png)

`Startup.cs`

```csharp
public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHangfire(configuration => configuration.UseSQLiteStorage());
        services.AddHangfireServer();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            var options = new DashboardOptions
            {
                DashboardTitle = "PDF Splitter",
                DisplayStorageConnectionString = false
            };

            endpoints.MapHangfireDashboard(string.Empty, options);
        });

        var options = new DailyCronJobOptions();
        IConfiguration configuration = app.ApplicationServices.GetService<IConfiguration>();
        configuration.GetSection(DailyCronJobOptions.DailyCronJob).Bind(options);
        string absoluteDirectoryPath = new FileInfo(options.DirectoryPath).FullName;

        RecurringJob.AddOrUpdate(
            () => CronJob.Scan(absoluteDirectoryPath),
            Cron.Daily(options.Hour, options.Minute));
    }
}
```

![Succeeded jobs](docs/succeeded-jobs.png)

`CronJob.cs`

```csharp
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
```

![CronJob.Scan](docs/cron-job_scan.png)

`Splitter.cs`

```csharp
public static class Splitter
{
    private const string InputFileNameSuffix = "_PB";
    private const string FileExtension = ".pdf";

    private static readonly string InputFileNameSuffixIncludingExtension = $"{InputFileNameSuffix}{FileExtension}";
    private static readonly string SearchPattern = $"*{InputFileNameSuffixIncludingExtension}";

    static Splitter() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public static string Split(string inputFilePath)
    {
        string outputFilePath = GetOutputFilePath(inputFilePath);
        using (PdfDocument input = PdfReader.Open(inputFilePath, PdfDocumentOpenMode.Import))
        {
            PdfPage firstPage = input.Pages[0];
            using (var output = new PdfDocument())
            {
                output.AddPage(firstPage);                    
                output.Save(outputFilePath);
            }
        }

        return outputFilePath;
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
```

![Splitter.Split](docs/splitter_split.png)

`SplitterTests.cs`

```csharp
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
```
