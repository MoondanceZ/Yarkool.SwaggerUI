using Swashbuckle.AspNetCore.SwaggerUI;

namespace Yarkool.SwaggerUI;

public class YarkoolSwaggerUIOptions : SwaggerUIOptions
{
    private const string _embeddedFileNamespace = "Yarkool.SwaggerUI.SwaggerUI";

    /// <summary>
    /// Embedded File Namespace
    /// </summary>
    public string EmbeddedFileNamespace { get; set; } = _embeddedFileNamespace;

    /// <summary>
    /// Gets or sets a Stream function for retrieving the swagger-ui page
    /// </summary>
    public new Func<Stream> IndexStream { get; set; } = () => typeof(YarkoolSwaggerUIOptions).Assembly.GetManifestResourceStream($"{_embeddedFileNamespace}.index.html");
}