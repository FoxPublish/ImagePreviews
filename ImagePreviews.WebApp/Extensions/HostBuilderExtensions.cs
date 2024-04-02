using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using ImagePreviews.WebApp.Util;

namespace ImagePreviews.WebApp.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureSerilog(
            this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                string seqServerUrl = context.Configuration["Log:SeqServerUrl"] ?? "";
                string seqApiKey = context.Configuration["Log:SeqApiKey"] ?? "";
                string logFolder = context.Configuration["Log:Folder"] ?? "";
                string logLevel = context.Configuration["Log:Level"] ?? "";
                var logLevelOveride = context.Configuration.GetSection("Log:Overrides").ToDictionary();

                string logFile = PathUtil.Combine(logFolder, "log-.txt");
                string logBufferFileName = PathUtil.Combine(logFolder, "serilogBuffer");
                bool writeToConsole = String.IsNullOrEmpty(seqServerUrl) || String.IsNullOrEmpty(seqApiKey);

                LoggingLevelSwitch levelSwitch = new()
                {
                    MinimumLevel = LogUtil.LevelFromString(logLevel)
                };

                loggerConfiguration
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .WriteTo.FileCustom(logFile, levelSwitch: levelSwitch)
                    .WriteTo.SeqCustom(serverUrl: seqServerUrl, apiKey: seqApiKey, bufferBaseFilename: logBufferFileName, controlLevelSwitch: levelSwitch)
                    .WriteTo.ConsoleCustom(writeToConsole);

                if (logLevelOveride != null && logLevelOveride.Count > 0)
                    loggerConfiguration.MinimumLevel.Overrides(logLevelOveride);
            });
        }
    }
}
