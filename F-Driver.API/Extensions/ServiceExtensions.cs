using F_Driver.API.Middleware;
using F_Driver.DataAccessObject.Models;
using F_Driver.Helpers;
using F_Driver.Repository;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using F_Driver.Service.Services;
using F_Driver.Service.Settings;
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
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "FLocalBrand API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });
            services.AddCors(option =>
                option.AddPolicy("CORS", builder =>
                    builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()));

            //services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            //Get config mail form environment
            services.Configure<MailSettings>(options =>
            {
                options.Server = Environment.GetEnvironmentVariable("MailSettings__Server");
                options.Port = int.Parse(Environment.GetEnvironmentVariable("MailSettings__Port") ?? "0");
                options.SenderName = Environment.GetEnvironmentVariable("MailSettings__SenderName");
                options.SenderEmail = Environment.GetEnvironmentVariable("MailSettings__SenderEmail");
                options.UserName = Environment.GetEnvironmentVariable("MailSettings__UserName");
                options.Password = Environment.GetEnvironmentVariable("MailSettings__Password");
            });


            services.Configure<FirebaseSettings>(config =>
            {
                config.ApiKey = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
                config.AuthEmail = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMAIL");
                config.AuthPassword = Environment.GetEnvironmentVariable("FIREBASE_AUTH_PASSWORD");
                config.Bucket = Environment.GetEnvironmentVariable("FIREBASE_BUCKET");
            });

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
            //Add Service
            return services.AddScoped<CancellationService>()
                .AddScoped<CancellationService>()
                .AddScoped<DriverService>()
                .AddScoped<FeedbackService>()
                .AddScoped<MessageService>()
                .AddScoped<PaymentService>()
                .AddScoped<PriceTableService>()
                .AddScoped<TransactionService>()
                .AddScoped<TripMatchService>()
                .AddScoped<UserService>()
                .AddScoped<VehicleService>()
                .AddScoped<WalletService>()
                .AddScoped<FeedbackService>()
                .AddScoped<ZoneService>()
           //Add repository
                .AddScoped<ICancellationReasonRepository,CancellationReasonRepository>()
                .AddScoped<ICancellationRepository, CancellationRepository>()
                .AddScoped<IDriverRepository, DriverRepository>()
                .AddScoped<IFeedbackRepository, FeedbackRepository>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<IPaymentRepository, PaymentRepository>()
                .AddScoped<IPriceTableRepository, PriceTableRepository>()
                .AddScoped<ITransactionRepository, TransactionRepository>()
                .AddScoped<ITripMatchRepository, TripMatchRepository>()
                .AddScoped<ITripRequestRepository, TripRequestRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IVehicleRepository, VehicleRepository>()
                .AddScoped<IWalletRepository, WalletRepository>()
                .AddScoped<IZoneRepository, ZoneRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                

                







                ;



        }
    }
}
