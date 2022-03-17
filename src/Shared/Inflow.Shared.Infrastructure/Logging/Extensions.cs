using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Logging.Decorators;
using Inflow.Shared.Infrastructure.Logging.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Inflow.Shared.Infrastructure.Logging;

public static class Extensions
{
    private const string ConsoleOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}";
    private const string FileOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";
    private const string AppSectionName = "app";
    private const string LoggerSectionName = "logger";

    public static IServiceCollection AddLoggingDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));

        return services;
    }

    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        app.Use(async (ctx, next) =>
        {
            var logger = ctx.RequestServices.GetRequiredService<ILogger<IContext>>();
            var context = ctx.RequestServices.GetRequiredService<IContext>();

            var path = ctx.Request.Path.Value;
            var method = ctx.Request.Method.ToUpper();
            var headers = ctx.Request.Headers;
            
            var headersBuilder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                headersBuilder.Append($"\t{key}:{value}{Environment.NewLine}");
            }

            logger.LogInformation(
                "Started processing a request {Method} {Path} with headers: {Headers} [Request ID: '{RequestId}', " +
                "Trace ID: '{TraceId}', User ID: '{UserId}']...",
                method, path, headersBuilder, context.RequestId, context.TraceId,
                context.IdentityContext.IsAuthenticated ? context.IdentityContext.Id : string.Empty);

            await next();

            logger.LogInformation(
                "Finished processing a request with status code: {StatusCode} [Request ID: '{RequestId}', Trace ID: '{TraceId}', User ID: '{UserId}']",
                ctx.Response.StatusCode, context.RequestId, context.TraceId,
                context.IdentityContext.IsAuthenticated ? context.IdentityContext.Id : string.Empty);
        });

        return app;
    }

    public static IHostBuilder UseLogging(this IHostBuilder builder, Action<LoggerConfiguration> configure = null,
        string loggerSectionName = LoggerSectionName, string appSectionName = AppSectionName)
        => builder.UseSerilog((ctx, loggerCfg) =>
        {
            if (string.IsNullOrWhiteSpace(loggerSectionName))
            {
                loggerSectionName = LoggerSectionName;
            }

            if (string.IsNullOrWhiteSpace(appSectionName))
            {
                appSectionName = AppSectionName;
            }

            var appOptions = ctx.Configuration.GetOptions<AppOptions>(appSectionName);
            var loggerOptions = ctx.Configuration.GetOptions<LoggerOptions>(loggerSectionName);

            MapOptions(loggerOptions, appOptions, loggerCfg, ctx.HostingEnvironment.EnvironmentName);
            configure?.Invoke(loggerCfg);
        });

    private static void MapOptions(LoggerOptions loggerOptions, AppOptions appOptions,
        LoggerConfiguration loggerConfiguration, string environment)
    {
        var level = GetLogEventLevel(loggerOptions.Level);

        loggerConfiguration.Enrich.FromLogContext()
            .MinimumLevel.Is(level)
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithProperty("Application", appOptions.Name)
            .Enrich.WithProperty("Instance", appOptions.Instance)
            .Enrich.WithProperty("Version", appOptions.Version);

        foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
        {
            loggerConfiguration.Enrich.WithProperty(key, value);
        }

        foreach (var (key, value) in loggerOptions.Overrides ?? new Dictionary<string, string>())
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions.ExcludePaths?.ToList().ForEach(x => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(x))));
        
        loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));
        
        Configure(loggerConfiguration, loggerOptions);
    }

    private static void Configure(LoggerConfiguration loggerConfiguration, LoggerOptions loggerOptions)
    {
        var consoleOptions = loggerOptions.Console ?? new ConsoleOptions();
        var fileOptions = loggerOptions.File ?? new FileOptions();
        var seqOptions = loggerOptions.Seq ?? new SeqOptions();

        if (consoleOptions.Enabled)
        {
            loggerConfiguration.WriteTo.Console(outputTemplate: ConsoleOutputTemplate);
        }

        if (fileOptions.Enabled)
        {
            var path = string.IsNullOrWhiteSpace(fileOptions.Path) ? "logs/logs.txt" : fileOptions.Path;
            if (!Enum.TryParse<RollingInterval>(fileOptions.Interval, true, out var interval))
            {
                interval = RollingInterval.Day;
            }

            loggerConfiguration.WriteTo.File(path, rollingInterval: interval, outputTemplate: FileOutputTemplate);
        }

        if (seqOptions.Enabled)
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
        }
    }

    private static LogEventLevel GetLogEventLevel(string level)
        => Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;
}