using Serilog;
using Serilog.Events;

namespace ImagePreviews.WebApp.Util
{
    public class LogUtil
    {
        public static void CloseAndFlush() => Log.CloseAndFlush();

        public static LogEventLevel LevelFromString(string level) => level.ToLower() switch
        {
            "trace" => LogEventLevel.Verbose, //.net level
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            "critical" => LogEventLevel.Fatal, //.net level
            _ => LogEventLevel.Information,
        };
    }
}
