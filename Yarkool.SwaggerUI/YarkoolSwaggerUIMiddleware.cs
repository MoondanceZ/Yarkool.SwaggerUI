﻿using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Yarkool.SwaggerUI;

public class YarkoolSwaggerUIMiddleware
{
    private static ConcurrentDictionary<string, string> swaggerJsonDic = new ConcurrentDictionary<string, string>();
    private readonly RequestDelegate _next;
    private readonly ISwaggerProvider _swaggerProvider;
    private readonly YarkoolSwaggerUIOptions _options;
    private readonly StaticFileMiddleware _staticFileMiddleware;
    private readonly SwaggerUIMiddleware _swaggerUiMiddleware;
    private readonly SwaggerMiddleware _swaggerMiddleware;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public YarkoolSwaggerUIMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnv, ILoggerFactory loggerFactory, ISwaggerProvider swaggerProvider, YarkoolSwaggerUIOptions swaggerUIOptions)
    {
        _next = next;
        _swaggerProvider = swaggerProvider;
        _options = swaggerUIOptions;

        _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, swaggerUIOptions);
        _swaggerUiMiddleware = CreateSwaggerUIMiddleware(next, hostingEnv, loggerFactory, swaggerUIOptions);

        var swaggerOptions = new SwaggerOptions();
        if (!swaggerOptions.RouteTemplate.StartsWith($"{swaggerUIOptions.RoutePrefix}/"))
        {
            swaggerOptions.RouteTemplate = swaggerOptions.RouteTemplate.Replace("swagger/", $"{swaggerUIOptions.RoutePrefix}/");
        }

        _swaggerMiddleware = CreateSwaggerMiddleware(next, swaggerOptions);

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
    }

    public async Task Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
    {
        var httpMethod = httpContext.Request.Method;
        var path = httpContext.Request.Path.Value;

        if (httpMethod == "GET" && !string.IsNullOrEmpty(path) && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$"))
        {
            await RespondWithIndexHtml(httpContext.Response);
            return;
        }

        if (httpMethod == "GET" && !string.IsNullOrEmpty(path) && (Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/assets/?") || Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/favicon.ico?")))
        {
            await _staticFileMiddleware.Invoke(httpContext);
            return;
        }

        if (httpMethod == "GET" && !string.IsNullOrEmpty(path) && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/v3/api-docs/swagger-config$"))
        {
            await RespondWithConfig(httpContext.Response);
            return;
        }

        if (httpMethod == "GET" && !string.IsNullOrEmpty(path) && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/swagger-resources$"))
        {
            await RespondWithConfigUrls(httpContext.Response);
            return;
        }

        if (httpMethod == "GET" && !string.IsNullOrEmpty(path) && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?.*/swagger.json$"))
        {
            if (swaggerJsonDic.TryGetValue(path, out var swaggerJson))
            {
                await RespondWithApplicationJson(httpContext.Response, swaggerJson);
                return;
            }
            else
            {
                using var memoryStream = new MemoryStream();
                var originalResponseBody = httpContext.Response.Body;
        
                try
                {
                    httpContext.Response.Body = memoryStream;
                    await _swaggerMiddleware.Invoke(httpContext, _swaggerProvider);
                    memoryStream.Position = 0;
            
                    using var reader = new StreamReader(memoryStream);
                    swaggerJson = await reader.ReadToEndAsync();
                    swaggerJsonDic.TryAdd(path, swaggerJson);
                    
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalResponseBody);
                }
                finally
                {
                    httpContext.Response.Body = originalResponseBody;
                }
        
                return;
            }
        }

        await _swaggerUiMiddleware.Invoke(httpContext);
    }

    private async Task RespondWithConfig(HttpResponse response)
    {
        await response.WriteAsync(JsonSerializer.Serialize(_options.ConfigObject, _jsonSerializerOptions));
    }

    private async Task RespondWithConfigUrls(HttpResponse response)
    {
        await response.WriteAsync(JsonSerializer.Serialize(_options.ConfigObject.Urls, _jsonSerializerOptions));
    }

    private async Task RespondWithIndexHtml(HttpResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "text/html;charset=utf-8";

        await using var stream = _options.IndexStream();
        // Inject arguments before writing to response
        if (stream != null)
        {
            var htmlBuilder = new StringBuilder(await new StreamReader(stream).ReadToEndAsync());

            foreach (var entry in GetIndexArguments())
            {
                htmlBuilder.Replace(entry.Key, entry.Value);
            }

            await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }
    }

    private async Task RespondWithApplicationJson(HttpResponse response, string json)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json;charset=utf-8";

        await response.WriteAsync(json, Encoding.UTF8);
    }

    private IDictionary<string, string> GetIndexArguments()
    {
        return new Dictionary<string, string>()
        {
            { "%(DocumentTitle)", _options.DocumentTitle },
            { "%(HeadContent)", _options.HeadContent },
        };
    }

    private StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnv, ILoggerFactory loggerFactory, YarkoolSwaggerUIOptions options)
    {
        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
            FileProvider = new EmbeddedFileProvider(typeof(YarkoolSwaggerUIMiddleware).GetTypeInfo().Assembly, _options.EmbeddedFileNamespace),
        };

        return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
    }

    private SwaggerUIMiddleware CreateSwaggerUIMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnv, ILoggerFactory loggerFactory, YarkoolSwaggerUIOptions options)
    {
        return new SwaggerUIMiddleware(next, hostingEnv, loggerFactory, options);
    }

    private SwaggerMiddleware CreateSwaggerMiddleware(RequestDelegate next, SwaggerOptions options)
    {
        return new SwaggerMiddleware(next, options);
    }
}