using System.Diagnostics;

namespace ImagePreviews.WebApp.Wrappers
{
    public class ProcessWrapper : IProcessWrapper
    {
        private readonly ILogger<ProcessWrapper> _logger;

        public ProcessWrapper(ILogger<ProcessWrapper> logger)
        {
            _logger = logger;
        }

        public bool Run(ProcessStartInfo startInfo, int timeout)
        {
            try
            {
                using var proc = new Process();
                proc.StartInfo = startInfo;
                proc.Start();

                if (!proc.WaitForExit(timeout))
                {
                    proc.Kill();
                    _logger.LogError("Failed to wait for ghostscript exit: {error}", proc.ExitCode);
                    return false;
                }

                var error = proc.StandardOutput.ReadToEnd();
                if (!String.IsNullOrEmpty(error))
                    _logger.LogError("gs error: {error}", error);

                proc.Dispose();
                proc.Close();
                proc.Refresh();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error running command. Exception: {ExceptionMessage}", ex.Message);
                return false;
            }

            return true;
        }
    }
}
