using System.Diagnostics;
using ImagePreviews.WebApp.Settings;
using ImagePreviews.WebApp.Util;
using ImagePreviews.WebApp.Wrappers;

namespace ImagePreviews.WebApp.Services
{
    public class GhostScript : IGhostScript
    {
        private readonly ILogger<GhostScript> _logger;
        private readonly IProcessWrapper _processWrapper;
        private readonly GhostScriptSettings _ghostScriptSettings;
        private readonly ImagePathSettings _imagePathSettings;

        public GhostScript(ILogger<GhostScript> logger, IProcessWrapper processWrapper, GhostScriptSettings ghostScriptSettings, ImagePathSettings imagePathSettings)
        {
            _logger = logger;
            _processWrapper = processWrapper;
            _ghostScriptSettings = ghostScriptSettings;
            _imagePathSettings = imagePathSettings;
        }

        public (string fileName, string outputFile) ConvertPageToImage(string path, string page, string imageFormat)
        {
            string device = GetDevice(imageFormat);
            string fileName = CreateImageFileName(path, page, imageFormat);
            string outputFile = PathUtil.Combine(_imagePathSettings.TempPath, fileName);
            string inputFile = PathUtil.Combine(_imagePathSettings.DataShareBasePath, path);

            string gsCommand = CreateCommand(device, page, outputFile, inputFile);

            if (!Run(gsCommand))
            {
                _logger.LogError("Could not convert {pdfFile} to {format}", path, imageFormat);
                return (String.Empty, String.Empty);
            }

            return (fileName, outputFile);
        }

        private static string GetDevice(string imgFormat) => imgFormat.ToLower() switch
        {
            "png" => "-sDEVICE=pngalpha",
            "jpg" => "-sDEVICE=jpeg",
            "jpeg" => "-sDEVICE=jpeg",
            _ => imgFormat
        };

        private static string CreateImageFileName(string path, string page, string imageFormat)
        {
            string pdfFileName = Path.GetFileNameWithoutExtension(path);
            return String.Format("{0}_{1}.{2}", pdfFileName, page, imageFormat);
        }

        private string CreateCommand(string device, string page, string outputFile, string inputFile)
        {
            return String.Format("{0} {1} -dFirstPage={2} -dLastPage={2} -sOutputFile=\"{3}\" \"{4}\"", _ghostScriptSettings.Variables, device, page, outputFile, inputFile);
        }

        private bool Run(string command)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = _ghostScriptSettings.PathExecutable,
                Arguments = command,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true
            };
            return _processWrapper.Run(startInfo, _ghostScriptSettings.TimeoutMs);
        }
    }
}
