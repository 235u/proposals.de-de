# Automatisches PDF-Splitting

S. https://www.twago.de/project/automatisches-pdf-splitting/163969/

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

![Succeeded jobs](docs/succeeded-jobs.png)

![CronJob.Scan](docs/cron-job_scan.png)

![Splitter.Split](docs/splitter_split.png)