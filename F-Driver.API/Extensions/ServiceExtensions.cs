using F_Driver.API.Middleware;
using F_Driver.DataAccessObject.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace F_Driver.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

          
        
            services.AddScoped<ExceptionMiddleware>();
            services.AddControllers();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddMemoryCache();
            services.AddEndpointsApiExplorer();
    
            //services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

            //services.Configure<CloundSettings>(configuration.GetSection(nameof(CloundSettings)));

            services.AddAuthorization();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureDbContext(configuration);

            // Configure Redis connection

            // Add StackExchangeRedisCache as the IDistributedCache implementation
            services.AddInfrastructureServices();
            // Add Mapper Services to Container injection

            return services;
        }
        private static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
            //var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            //var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            //var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            //var dbTrustServerCertificate = Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE");
            //var dbMultipleActiveResultSets = Environment.GetEnvironmentVariable("DB_MULTIPLE_ACTIVE_RESULT_SETS");
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");
            //var connectionString = $"Data Source={dbServer};Initial Catalog={dbName};User ID={dbUser};Password={dbPassword};TrustServerCertificate={dbTrustServerCertificate};MultipleActiveResultSets={dbMultipleActiveResultSets}";

            services.AddDbContext<FDriverContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {

            return services;



        }
    }
}
