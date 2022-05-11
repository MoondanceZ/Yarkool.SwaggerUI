using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Yarkool.SwaggerUI;

public class YarkoolSwaggerUIOptions : SwaggerUIOptions
{
    public YarkoolSwaggerUIOptions()
    {
        PreSerializeFilters = new List<Action<OpenApiDocument, HttpRequest>>();
        SerializeAsV2 = false;
    }

    /// <summary>
    /// Embedded File Namespace
    /// </summary>
    public const string EmbeddedFileNamespace = "Yarkool.SwaggerUI.SwaggerUI";

    /// <summary>
    /// Sets a custom route for the Swagger JSON/YAML endpoint(s). Must include the {documentName} parameter
    /// </summary>
    public string RouteTemplate { get; set; } = "swagger/{documentName}/swagger.{json|yaml}";

    /// <summary>
    /// Return Swagger JSON/YAML in the V2 format rather than V3
    /// </summary>
    public bool SerializeAsV2 { get; set; }

    /// <summary>
    /// Actions that can be applied to an OpenApiDocument before it's serialized.
    /// Useful for setting metadata that's derived from the current request
    /// </summary>
    public List<Action<OpenApiDocument, HttpRequest>> PreSerializeFilters { get; private set; }

    /// <summary>
    /// Gets or sets a Stream function for retrieving the swagger-ui page
    /// </summary>
    public new Func<Stream> IndexStream { get; set; } = () => typeof(YarkoolSwaggerUIOptions).Assembly.GetManifestResourceStream($"{EmbeddedFileNamespace}.index.html");
}