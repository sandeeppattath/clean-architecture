namespace SampleProject.API.Extensions
{
    public static class CorsExtension
    {
        public static void AddCorsPolicyExtension(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });
        }
    }
}
