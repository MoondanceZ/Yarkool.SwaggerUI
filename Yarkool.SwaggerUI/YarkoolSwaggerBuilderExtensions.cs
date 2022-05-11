using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Yarkool.SwaggerUI;

public static class YarkoolSwaggerBuilderExtensions
{
    /// <summary>
    /// Register the Swagger middleware with provided options
    /// </summary>
    public static IApplicationBuilder UseYarkoolSwaggerUI(this IApplicationBuilder app, Action<YarkoolSwaggerUIOptions> setupSwaggerUIAction = null)
    {
        var swaggerUiOptions = app.ApplicationServices.GetRequiredService<IOptions<YarkoolSwaggerUIOptions>>().Value ?? new YarkoolSwaggerUIOptions();
        setupSwaggerUIAction?.Invoke(swaggerUiOptions);

        app.UseMiddleware<YarkoolSwaggerUIMiddleware>(swaggerUiOptions);

        return app;
    }
}