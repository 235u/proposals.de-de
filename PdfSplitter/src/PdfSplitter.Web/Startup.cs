using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PdfSplitter.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration.UseSQLiteStorage());
            services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard(string.Empty);
            });

            var options = new DailyCronJobOptions();
            IConfiguration configuration = app.ApplicationServices.GetService<IConfiguration>();
            configuration.GetSection(DailyCronJobOptions.DailyCronJob).Bind(options);

            RecurringJob.AddOrUpdate(
                () => Scan(options.DirectoryPath),
                Cron.Daily(options.Hour, options.Minute));
        }

        public void Scan(string directoryPath)
        {
            foreach (string inputFilePath in Splitter.GetInputFilePaths(directoryPath))
            {
                BackgroundJob.Enqueue(() => Splitter.Split(inputFilePath));
            }
        }
    }
}
