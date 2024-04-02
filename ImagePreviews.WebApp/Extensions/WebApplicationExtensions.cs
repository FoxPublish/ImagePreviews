using Imageflow.Fluent;
using Imageflow.Server;
using ImagePreviews.WebApp.Middleware;
using ImagePreviews.WebApp.Settings;

namespace ImagePreviews.WebApp.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void ConfigurePdfMethod(this WebApplication app, ConfigurationManager configManager)
        {
            var pdfRedirect = configManager.GetSection("PdfRedirect").Get<PdfRedirectSettings>();
            if (pdfRedirect.Active)
            {
                app.MapWhen(context => context.Request.Path.HasValue && context.Request.Path.Value.ToLower().EndsWith(".pdf"), app =>
                {
                    app.Run(context =>
                    {
                        context.Response.Redirect(pdfRedirect.Url + context.Request.Path.Value + context.Request.QueryString.Value);
                        return Task.CompletedTask;
                    });
                });
            }
            else
            {
                app.UseMiddleware<PdfMiddleware>();
            }
        }

        public static void ConfigureImageFlow(this WebApplication app, ConfigurationManager configManager)
        {
            var sizeLimitSettings = configManager.GetSection("FrameSizeLimit").Get<FrameSizeLimitSettings>();
            var imagePathSettings = configManager.GetSection("ImagePaths").Get<ImagePathSettings>();
            var projectSettings = configManager.GetSection("Project").Get<ProjectSettings>();

            var sizeLimit = new FrameSizeLimit(
                sizeLimitSettings.Width,
                sizeLimitSettings.Height,
                sizeLimitSettings.Megapixel);

            app.UseImageflow(new ImageflowMiddlewareOptions()
                .MapPath("", imagePathSettings.DataShareBasePath)
                .MapPath("/temp", imagePathSettings.TempPath)
                .SetMyOpenSourceProjectUrl(projectSettings.Url)
                .SetAllowCaching(true)
                .AddCommandDefault("down.filter", "mitchell")
                .AddCommandDefault("f.sharpen", "15")
                .AddCommandDefault("webp.quality", "90")
                .AddPreset(new PresetOptions("large", PresetPriority.DefaultValues)
                    .SetCommand("width", "1024")
                    .SetCommand("height", "1024")
                    .SetCommand("mode", "max"))
                .SetJobSecurityOptions(new SecurityOptions()
                    .SetMaxDecodeSize(sizeLimit)
                    .SetMaxFrameSize(sizeLimit)
                    .SetMaxEncodeSize(sizeLimit)
                ));
        }
    }
}
