using Hangfire;
using Hangfire.SqlServer;
using Owin;
using SimpleInjector;
using System;
using System.Configuration;

namespace OpenTicket.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new Container();
            DependencyConfig.Bootstrap(container);
            ConfigureHangfire(app, container);
        }

        private void ConfigureHangfire(IAppBuilder app, Container container)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Main"].ConnectionString;
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseActivator(new HangfireSimpleInjectorActivator(container))
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                });
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            if (!int.TryParse(ConfigurationManager.AppSettings["EmailImportInterval"], out int interval))
                throw new ConfigurationErrorsException("No valid email import job interval found: EmailImportInterval");
            RecurringJob.AddOrUpdate<ImportEmailJob>(ImportEmailJob.Name, job => job.ExecuteAsync(null),
                $"*/{interval} * * * *");
        }
    }
}