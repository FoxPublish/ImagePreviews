using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;
using Imageflow.Server.HybridCache;
using ImagePreviews.WebApp.Settings;

namespace ImagePreviews.WebApp.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSettings(this IServiceCollection services)
        {
            services.AddOption<GhostScriptSettings>("Ghostscript");
            services.AddOption<ImagePathSettings>("ImagePaths");
            services.AddOption<CacheSettings>("Cache");
            services.AddOption<FrameSizeLimitSettings>("FrameSizeLimit");
            services.AddOption<PdfRedirectSettings>("PdfRedirect");
            services.AddOption<ProjectSettings>("Project");
			services.AddOption<RateLimitSettings>("RateLimit");

			services.AddSingleton(r => r.GetRequiredService<IOptions<GhostScriptSettings>>().Value);
            services.AddSingleton(r => r.GetRequiredService<IOptions<ImagePathSettings>>().Value);
        }

        private static void AddOption<TOption>(this IServiceCollection services, string configSection) where TOption : class
        {
            services.AddOptions<TOption>()
                .BindConfiguration(configSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        public static void ConfigureHybridCache(this IServiceCollection services, ConfigurationManager configManager)
        {
            var cache = configManager.GetSection("Cache").Get<CacheSettings>();
            var hybridCacheOptions = new HybridCacheOptions(cache.Dir)
            {
                // How long after a file is created before it can be deleted
                MinAgeToDelete = TimeSpan.FromSeconds(cache.MinAgeToDelete),
                // How much RAM to use for the write queue before switching to synchronous writes
                WriteQueueMemoryMb = cache.WriteQueueMemoryMb,
                // The maximum size of the cache
                CacheSizeMb = cache.CacheSizeMb
            };

            services.AddImageflowHybridCache(hybridCacheOptions);
        }

        public static void ConfigureRateLimiter(this IServiceCollection services, ConfigurationManager configManager)
        {
            var settings = configManager.GetSection("RateLimit").Get<RateLimitSettings>();
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                    RateLimitPartition.GetSlidingWindowLimiter(context.Connection.RemoteIpAddress!, _ =>
					    new SlidingWindowRateLimiterOptions
					    {
						    AutoReplenishment = true,
						    PermitLimit = settings.PermitLimit,
						    Window = TimeSpan.FromSeconds(settings.WindowTimeInSeconds),
						    SegmentsPerWindow = settings.SegmentsPerWindow,
						    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						    QueueLimit = settings.QueueLimit
					    }));

				options.OnRejected = async (context, token) =>
				{
					var logger = context.HttpContext.RequestServices.GetService<ILogger<SlidingWindowRateLimiter>>();

                    logger?.LogWarning("OnRejected: Too many requests by {IpAddress} on endpoint {Path}{Query}",
                        context.HttpContext.Connection.RemoteIpAddress,
                        context.HttpContext.Request.Path,
                        context.HttpContext.Request.QueryString);

					await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
				};
			});
        }
    }
}
