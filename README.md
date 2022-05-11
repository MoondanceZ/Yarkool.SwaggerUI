Yarkool.SwaggerUI

![image](https://user-images.githubusercontent.com/11675776/167785105-50f9ca51-6e46-442d-b9bc-b0a07c0ff03e.png)


```C#
//Add swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });
    options.AddServer(new OpenApiServer()
    {
        Url = "",
        Description = "Description"
    });
    options.CustomOperationIds(apiDesc =>
    {
        var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
        return controllerAction!.ControllerName + "-" + controllerAction.ActionName;
    });

    foreach (var file in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
    {
        options.IncludeXmlComments(file);
    }

    var securityScheme = new OpenApiSecurityScheme
    {
        Description = "Format: Bearer {access_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.ApiKey
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer", //The name of the previously defined security scheme.
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});
```

```C#
// app.UseSwagger()
app.UseYarkoolSwagger(c =>
{
    c.SwaggerEndpoint("/v1/swagger.json", "V1 Docs");
});

app.UseRouting().UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapSwagger();
});
```
