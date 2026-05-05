using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplication1.WebApi.Extensions
{
    /// <summary>
    /// ustawienia swaggera, aby można było używać jwt tokena
    /// </summary>
    public static class SwaggerExtension
    {
        /// <summary>
        /// ustawienia swaggera, aby można było używać jwt tokena
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSwagger(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSwaggerGen(options =>
            {
                // Specify OpenAPI 3.x version here
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "webapi",
                    Version = "v1"
                });

                // Explicitly set the OpenAPI version
                //options.AddServer(new OpenApiServer { Url = configuration["Swagger:Url"] }); // Optional server URL

                options.CustomOperationIds(apiDesc =>
                    apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null
                );

                // Security Definition for JWT Bearer
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token in format: Bearer {token}"
                });

                // Security Requirement for JWT Bearer
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Include XML comments if the XML file is found
                var xmlFile = "WebApplication1.WebApi.xml";

                var dir = new DirectoryInfo(AppContext.BaseDirectory);

                while (dir != null && !File.Exists(Path.Combine(dir.FullName, xmlFile)))
                {
                    dir = dir.Parent;
                }

                var xmlPath = dir == null ? null : Path.Combine(dir.FullName, xmlFile);

                if (!string.IsNullOrEmpty(xmlPath))
                    options.IncludeXmlComments(xmlPath);

                // Custom operation filter (optional)
                options.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();
            });
        }
    }
}
