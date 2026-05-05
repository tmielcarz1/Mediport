//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using WebApplication1.Application.Configuration;

//namespace WebApplication1.WebApi.Extensions
//{
//    public static class AccountExtension
//    {
//        public static void AddAuthentication(this IServiceCollection services, ConfigurationManager configuration)
//        {
//            var accountConfig = configuration.GetSection("AccountConfiguration").Get<AccountConfiguration>();
//            services.AddSingleton(accountConfig);

//            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                .AddJwtBearer(options =>
//                {
//                    options.RequireHttpsMetadata = false;
//                    options.SaveToken = true;

//                    options.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuer = true,
//                        ValidateAudience = true,
//                        ValidateLifetime = true,
//                        ValidateIssuerSigningKey = true,

//                        ValidIssuer = accountConfig.JwtIssuer,
//                        ValidAudience = accountConfig.JwtIssuer,
//                        IssuerSigningKey = new SymmetricSecurityKey(
//                            Encoding.UTF8.GetBytes(accountConfig.JwtKey))
//                    };
//                });
//        }

//        public static void AddAuthorization(this IServiceCollection services)
//        {
//            services.AddAuthorization(options =>
//            {
//                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
//                    .RequireAuthenticatedUser()
//                    .Build();

//                options.InvokeHandlersAfterFailure = true;
//            });
//        }


//    }
//}
