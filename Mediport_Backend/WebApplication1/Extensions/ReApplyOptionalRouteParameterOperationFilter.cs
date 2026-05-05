using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;

namespace WebApplication1.WebApi.Extensions
{
    /// <summary>
    /// poprawki dla swaggera, aby były widoczne opcjonalne parametry (route)
    /// </summary>
    public class ReApplyOptionalRouteParameterOperationFilter : IOperationFilter
    {
        const string captureName = "routeParameter";

        /// <summary>
        /// poprawki dla swaggera, aby były widoczne opcjonalne parametry (route)
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var httpMethodAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();

            var httpMethodWithOptional = httpMethodAttributes?.FirstOrDefault(m => m.Template?.Contains("?") ?? false);
            if (httpMethodWithOptional == null)
                return;

            string regex = $"{{(?<{captureName}>\\w+)\\?}}";

            var template = httpMethodWithOptional.Template;
            if (template == null)
                return;

            var matches = Regex.Matches(template, regex);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var name = match.Groups[captureName].Value;

                var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
                if (parameter != null)
                {
                    parameter.AllowEmptyValue = true;
                    parameter.Description = "Must check \"Send empty value\" or Swagger passes a comma for empty values otherwise";
                    parameter.Required = false;
                    //parameter.Schema.Default = new OpenApiString(string.Empty);
                    parameter.Schema.Nullable = true;
                }
            }
        }
    }
}
