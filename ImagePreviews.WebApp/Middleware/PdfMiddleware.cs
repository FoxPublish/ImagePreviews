using System.IO.Abstractions;
using ImagePreviews.WebApp.Services;
using ImagePreviews.WebApp.Util;

namespace ImagePreviews.WebApp.Middleware
{
    public class PdfMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PdfMiddleware> _logger;
        
        public PdfMiddleware(RequestDelegate next, ILogger<PdfMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IFileSystem fileSystem, IGhostScript ghostscript)
        {
            string tempImage = String.Empty;

            if (IsValidPdfRequest(context))
            {
                var (path, page, imageFormat) = GetRequestInfo(context);
                (string imageName, tempImage) = ghostscript.ConvertPageToImage(path, page, imageFormat);
                SetTempRequestPath(context, imageName);
            }

            await _next.Invoke(context);

            if (!String.IsNullOrEmpty(tempImage) && fileSystem.File.Exists(tempImage))
                fileSystem.File.Delete(tempImage);
        }

        private bool IsValidPdfRequest(HttpContext context)
        {
            if (!context.Request.Path.HasValue)
                return false;

            var extension = Path.GetExtension(context.Request.Path.Value);
            if (!extension.ToLower().EndsWith(".pdf"))
                return false;

            if (context.Request.Query.Count <= 0)
            {
                _logger.LogError("Cannot convert pdf, missing query - {Path}", context.Request.Path.Value);
                return false;
            }

            if (String.IsNullOrEmpty(context.Request.Query["format"]))
            {
                _logger.LogError("Cannot convert pdf, missing format - {Path}{Query}", context.Request.Path.Value, context.Request.QueryString);
                return false;
            }

            return true;
        }

        private static (string path, string page, string imageFormat) GetRequestInfo(HttpContext context)
        {
            var page = context.Request.Query["page"].ToString();
            var path = context.Request.Path.Value ?? "";
            var format = context.Request.Query["format"].ToString();

            if (String.IsNullOrEmpty(page))
                page = "1";

            return (path, page, format);
        }

        private static void SetTempRequestPath(HttpContext context, string fileName)
        {
            context.Request.Path = new PathString("/temp/" + fileName);
        }
    }
}
