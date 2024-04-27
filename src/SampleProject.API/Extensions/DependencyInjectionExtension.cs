using System.Data.Common;

namespace SampleProject.API.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjectionServiceExtension(this IServiceCollection services)
        {

            //services.AddScoped<ITokenProvider, TokenProvider.TokenProvider>();

        }
    }
}
