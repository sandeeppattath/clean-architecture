using Microsoft.Extensions.Configuration;

namespace Common.HelperLibrary.Helpers
{

    public static class AppSettingsHelper
    {
        public static IConfiguration? config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
        }
        public static string GetValueByKey(string Key)
        {
            return config?.GetSection(Key).Value;
        }

        public static string? GetConnectionString(string Key)
        {
            return config?.GetConnectionString(Key) ?? "";
        }
    }
}
