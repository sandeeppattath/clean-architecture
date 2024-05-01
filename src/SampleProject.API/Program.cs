using Autofac.Extensions.DependencyInjection;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SampleProject.Infrastructure.Data.DbContext;
using SampleProject.Infrastructure.Data.DbContext.Configuration;

namespace SampleProject.API
{
    public class Program
    {
        public static readonly string? Namespace = typeof(Program).Namespace;
        public static readonly string? AppName = Namespace?.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            SetLogBaseDirectory();

            var configuration = GetConfiguration();

            try
            {
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    Log.Information("Starting web host ({ApplicationContext})", AppName);

                    // add-migration -name Initial -context AppDbContext -outputDir Data\Migrations\DbContextMigrations -verbose
                    var appDBContext = services.GetRequiredService<AppDbContext>();
                    appDBContext.Database.Migrate();
                    AppDbContextSeed.SeedAsync(appDBContext, loggerFactory, services).Wait();

                    // add-migration -name InitialIdentityServerConfigurationDBMigration -context ConfigurationDbContext -outputDir Data\Migrations\ConfigurationDBMigrations -verbose
                    var configurationDBContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    configurationDBContext.Database.Migrate();
                    ConfigurationDbContextSeed.SeedAsync(configurationDBContext, loggerFactory, configuration).Wait();

                    // add-migration -name InitialIdentityServerOperationalDBMigration -context PersistedGrantDBContext -outputDir Data\Migrations\PersistedGrantDBMigrations -verbose
                    scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                }
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void SetLogBaseDirectory()
        {
            Environment.SetEnvironmentVariable("BASEDIR", AppContext.BaseDirectory);
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            return builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            // Add Keyvault Here
            .UseSerilog()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}