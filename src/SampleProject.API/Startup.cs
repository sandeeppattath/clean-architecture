using Common.HelperLibrary.Helpers;
using Common.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SampleProject.API.Extensions;
using SampleProject.ApplicationCore.Entities;
using SampleProject.Infrastructure.Data.DbContext;
using System.Reflection;

namespace SampleProject.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private const string EmailConfirmationTokenProviderName = "ConfirmEmail";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettingsHelper.Initialize(Configuration);
            var issuerURL = AppSettingsHelper.GetValueByKey("IssuerUrl");

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.AddControllers().AddNewtonsoftJson();

            // Added through Extensions\SwaggerExtension.cs
            services.AddSwaggerToServiceExtension();

            // Added through Extensions\DependencyInjectionExtension.cs
            services.AddDependencyInjectionServiceExtension();
            var db = Configuration["DemoDBConnectionString"];
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseSqlServer(Configuration["DemoDBConnectionString"]);
            });

            //services.AddDbContext<IntegrationEventLogContext>(options =>
            //    options.UseSqlServer(Configuration["IdentityDBConnectionString"],
            //    sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name)));

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
                options.Tokens.PasswordResetTokenProvider = EmailConfirmationTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(Convert.ToDouble(Configuration.GetSection("VerificationMailExpiry").Value));
            });

            // AddIdentity registers the services
            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Password.RequiredLength = 12;
                config.Password.RequireDigit = true;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequireUppercase = true;
                config.Password.RequireLowercase = true;
                config.User.RequireUniqueEmail = true;
                config.SignIn.RequireConfirmedEmail = true;
                config.User.RequireUniqueEmail = false;
            })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders()
                    .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<ApplicationUser>>(EmailConfirmationTokenProviderName);

            AddIdentityServer(services, issuerURL);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            LifetimeValidator = (notBefore, expires, securityToken, validationParameter) =>
                              expires >= DateTime.UtcNow.AddMinutes(-60)
                        };
                        options.RequireHttpsMetadata = false;
                        options.Authority = issuerURL;
                    });

            services.AddHttpContextAccessor();

            // Added through Extensions\CorsExtesnions.cs
            services.AddCorsPolicyExtension();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSerilogRequestLogging(); //For logging the API requests
                                            //  app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            UseSwagger(app);
            app.UseIdentityServer();
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static void UseSwagger(IApplicationBuilder app)
        {
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        private IServiceCollection AddIdentityServer(IServiceCollection services, string issuerURL)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var identityServerBuilder = services.AddIdentityServer(options =>
            {
                options.IssuerUri = issuerURL;
                options.Events.RaiseSuccessEvents = true;
                options.KeyManagement.Enabled = false;
            });

            identityServerBuilder.AddAspNetIdentity<ApplicationUser>();
            if (environment?.ToLower() == "development" || environment?.ToLower() == "testing")
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            //else
            //{
            //var client = new CertificateClient(new Uri(Configuration["KeyVaultName"]), new DefaultAzureCredential());
            //X509Certificate2 certificate = client.DownloadCertificate("certificate");
            //identityServerBuilder.AddSigningCredential(certificate);
            //}

            identityServerBuilder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                builder.UseSqlServer(Configuration["DemoDBConnectionString"],
                options => options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
            });

            identityServerBuilder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                builder.UseSqlServer(Configuration["DemoDBConnectionString"],
                options => options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
            });

            // Profile Service (if any) can be added here
            identityServerBuilder.AddServerSideSessions();
            return services;
        }

    }

    public class ConfirmEmailDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public ConfirmEmailDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider
            , IOptions<ConfirmEmailDataProtectionTokenProviderOptions> options
            , ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
        }
    }

    public class ConfirmEmailDataProtectionTokenProviderOptions : DataProtectionTokenProviderOptions { }
}

