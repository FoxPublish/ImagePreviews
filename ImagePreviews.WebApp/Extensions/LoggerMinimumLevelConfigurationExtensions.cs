using Serilog;
using Serilog.Configuration;
using ImagePreviews.WebApp.Util;

namespace ImagePreviews.WebApp.Extensions
{
    public static class LoggerMinimumLevelConfigurationExtensions
    {
        public static LoggerConfiguration Overrides(
            this LoggerMinimumLevelConfiguration minLevelConfiguration, Dictionary<string, string> section)
        {
            LoggerConfiguration? loggerConfig = null;
            foreach (var pair in section)
            {
                loggerConfig = minLevelConfiguration.Override(pair.Key, LogUtil.LevelFromString(pair.Value));
            }
            return loggerConfig ?? minLevelConfiguration.Override("null", Serilog.Events.LogEventLevel.Fatal);
        }
    }
}
