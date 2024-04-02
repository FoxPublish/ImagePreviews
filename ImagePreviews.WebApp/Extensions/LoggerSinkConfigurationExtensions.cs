using Serilog;
using Serilog.Configuration;
using Serilog.Core;

namespace ImagePreviews.WebApp.Extensions
{
    public static class LoggerSinkConfigurationExtensions
    {
        const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024; // 1GB
        const string DefaultOutputTemplate = "{Timestamp:HH:mm:ss.fff} [{Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}";
        const int DefaultRetainedFileCountLimit = 100;
        const string DummyAddress = "http://0.0.0.0";

        public static LoggerConfiguration FileCustom(
            this LoggerSinkConfiguration sinkConfiguration,
            string logFilePath,
            string outputTemplate = DefaultOutputTemplate,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            LoggingLevelSwitch? levelSwitch = null,
            RollingInterval rollingInterval = RollingInterval.Day,
            bool rollOnFileSizeLimit = true,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit)
        {
            return sinkConfiguration.File(
                logFilePath,
                outputTemplate: outputTemplate,
                fileSizeLimitBytes: fileSizeLimitBytes,
                levelSwitch: levelSwitch,
                rollingInterval: rollingInterval,
                rollOnFileSizeLimit: rollOnFileSizeLimit,
                retainedFileCountLimit: retainedFileCountLimit);
        }

        public static LoggerConfiguration SeqCustom(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string apiKey,
            string bufferBaseFilename,
            LoggingLevelSwitch? controlLevelSwitch)
        {
            bool writeToSeq = !string.IsNullOrEmpty(serverUrl) && !string.IsNullOrEmpty(apiKey);

#if DEBUG
            writeToSeq = false;
#endif

            return sinkConfiguration.Conditional(
                evt => writeToSeq,
                wt => wt.Seq(
                    serverUrl: writeToSeq ? serverUrl : DummyAddress,
                    apiKey: apiKey,
                    bufferBaseFilename: bufferBaseFilename,
                    controlLevelSwitch: controlLevelSwitch));
        }

        public static LoggerConfiguration ConsoleCustom(
            this LoggerSinkConfiguration sinkConfiguration,
            bool writeToConsole)
        {
            return sinkConfiguration.Conditional(
                evt => writeToConsole,
                wt => wt.Console());
        }
    }
}
