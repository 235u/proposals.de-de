using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace PdfSplitter.Web
{
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
}
