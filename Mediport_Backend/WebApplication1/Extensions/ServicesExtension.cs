using WebApplication1.Application.Configuration;
using WebApplication1.Application.Services.Tags;

namespace WebApplication1.WebApi.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServicesExtension
    {
        /// <summary>
        /// AddApplication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddApplication(this IServiceCollection services, ConfigurationManager configuration)
        {
            //pozostałe
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSwagger(configuration);
            services.AddAutoMapper(cfg =>
            {
                cfg.AllowNullCollections = false;
                cfg.AllowNullDestinationValues = true;
            }, AppDomain.CurrentDomain.GetAssemblies());


            //configi
            var stackOverflowApiConfiguration = configuration.GetSection("StackOverflowApiConfiguration").Get<StackOverflowApiConfiguration>();
            services.AddSingleton(stackOverflowApiConfiguration!);


            //serwisy
            services.AddScoped<ITagsService, TagsService>();


        }




    }
}
