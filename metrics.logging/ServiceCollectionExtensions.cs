using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace metrics.logging
{
    public static class ServiceCollectionExtensions
    {
        public static ILoggingBuilder AddMetricsLogging(this ILoggingBuilder builder, IConfiguration configuration)
        {
            ConfigSettingLayoutRenderer.DefaultConfiguration = configuration;
            builder.ClearProviders();

            var logConfig = new NLogLoggingConfiguration(configuration.GetSection("NLog"));
            LogManager.Configuration = logConfig;

            builder.AddProvider(new NLogLoggerProvider(NLogAspNetCoreOptions.Default, new LogFactory(logConfig)));
            builder.SetMinimumLevel(LogLevel.Trace);

            return builder;
        }
    }
}