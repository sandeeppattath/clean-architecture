using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Duende.IdentityServer.EntityFramework.Mappers;
using Serilog;

namespace SampleProject.Infrastructure.Data.DbContext.Configuration
{
    public class ConfigurationDbContextSeed
    {
        public class Config
        {
            private static readonly string HospitalScope = "hospitalscope";
            public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
            }
            public static IEnumerable<ApiScope> GetApiScopes()
            {
                return new List<ApiScope>
            {
                // shared scope
                new ApiScope(name: "hospitalscope", displayName: "hospitalscope"),
                new ApiScope(name: "hospital", displayName: "Hospital"),
             };
            }
            public static IEnumerable<ApiResource> GetApiResources()
            {
                return new List<ApiResource>
            {
                new ApiResource("hospital", "Hospital")
                {
                      Scopes = { "hospitalscope" }
                },
            };
            }

            public static IEnumerable<Client> GetClients(IConfiguration Configuration)
            {
                var clientIDReactSPA = Configuration.GetValue<string>("ClientIDReactSPA");
                var clientSecretReactSPA = Configuration.GetValue<string>("ClientSecretReactSPA");
                var accessTokenLifeTime = Configuration.GetValue<int>("AccessTokenLifeTime");
                var refreshTokenLifeTime = Configuration.GetValue<int>("RefreshTokenLifeTime");

                return new[]
                {
                new Client
                {
                  ClientName = "reactSPA with Resource Owner Password Flow",
                  ClientId = clientIDReactSPA,
                  AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                  ClientSecrets =
                  {
                    new Secret(clientSecretReactSPA.Sha256())
                  },
                  AllowedScopes =
                  {
                    HospitalScope,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.OfflineAccess
                  },
                  AllowOfflineAccess = true,
                  RefreshTokenUsage = TokenUsage.OneTimeOnly,
                  AccessTokenLifetime = accessTokenLifeTime,
                  RefreshTokenExpiration = TokenExpiration.Absolute,
                  AbsoluteRefreshTokenLifetime = refreshTokenLifeTime,
                  AlwaysIncludeUserClaimsInIdToken = true
                },

            };
            }



        }

        public static async Task SeedAsync(ConfigurationDbContext configurationDBContext,
               ILoggerFactory loggerFactory, IConfiguration configuration, int? retry = 0)
        {
            try
            {
                Log.Information("Starting seeding Configuration database. . .");
                if (!Config.GetClients(configuration).Any())
                {
                    var clients = Config.GetClients(configuration);
                    foreach (var client in clients)
                    {
                        configurationDBContext.Clients.Add(client.ToEntity());
                    }
                    await configurationDBContext.SaveChangesAsync();
                }
                else
                {
                    var clients = Config.GetClients(configuration);
                    foreach (var item in clients)
                    {
                        var clientDetail = configurationDBContext.Clients.FirstOrDefault(m => m.ClientName == item.ClientName);
                        if (clientDetail == null)
                        {
                            configurationDBContext.Clients.Add(item.ToEntity());
                            await configurationDBContext.SaveChangesAsync();
                        }
                    }
                }
                if (!configurationDBContext.ApiResources.Any())
                {
                    var apiResources = Config.GetApiResources();
                    foreach (var apiResource in apiResources)
                    {
                        configurationDBContext.ApiResources.Add(apiResource.ToEntity());
                    }
                    await configurationDBContext.SaveChangesAsync();
                }
                if (!configurationDBContext.IdentityResources.Any())
                {
                    var identityResources = Config.GetIdentityResources();
                    foreach (var identityResource in identityResources)
                    {
                        configurationDBContext.IdentityResources.Add(identityResource.ToEntity());
                    }
                    await configurationDBContext.SaveChangesAsync();
                }
                if (!configurationDBContext.ApiScopes.Any())
                {
                    var apiScopes = Config.GetApiScopes();
                    foreach (var apiScope in apiScopes)
                    {
                        configurationDBContext.ApiScopes.Add(apiScope.ToEntity());
                    }
                    await configurationDBContext.SaveChangesAsync();
                }

                Log.Information("Done seeding Configuration database. Exiting. .");
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<AppDbContext>();
                logger.LogError(ex, "An error occurred seeding Configuration DB tables.");
            }

        }
    }
}
