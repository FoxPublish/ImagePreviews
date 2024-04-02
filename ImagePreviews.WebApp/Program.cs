using ImagePreviews.WebApp.Extensions;
using ImagePreviews.WebApp.Services;
using ImagePreviews.WebApp.Util;
using ImagePreviews.WebApp.Wrappers;
using System.IO.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilog();
builder.Services.AddHealthChecks();
builder.Services.ConfigureSettings();
builder.Services.AddScoped<IGhostScript, GhostScript>();
builder.Services.AddScoped<IProcessWrapper, ProcessWrapper>();
builder.Services.AddScoped<IFileSystem, FileSystem>();
builder.Services.ConfigureHybridCache(builder.Configuration);
builder.Services.ConfigureRateLimiter(builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/alert/ishealthy");
app.UseRouting();

app.UseRateLimiter();
app.ConfigurePdfMethod(builder.Configuration);
app.ConfigureImageFlow(builder.Configuration);

app.Run();

LogUtil.CloseAndFlush();