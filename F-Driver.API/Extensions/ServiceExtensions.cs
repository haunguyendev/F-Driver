using F_Driver.API.Middleware;
using F_Driver.DataAccessObject.Models;
using F_Driver.Helpers;
using F_Driver.Repository;
using F_Driver.Repository.Interfaces;
using F_Driver.Repository.Repositories;
using F_Driver.Service.Mapper;
using F_Driver.Service.Services;
using F_Driver.Service.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace F_Driver.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

          
        
            services.AddScoped<ExceptionMiddleware>();
            services.AddControllers()
                    .AddJsonOptions(x =>
                        {
                            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                            x.JsonSerializerOptions.Encoder=System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                            }); // Ngăn không tuần tự hóa vòng lặp

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(config =>
            {
                config.EnableAnnotations();
            });

            var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured.");
            }
            var jwtSettings = new JwtSettings
            {
                Key = secretKey
            };
            services.Configure<JwtSettings>(options => { options.Key=jwtSettings.Key; });

            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("ClientID is not configured.");
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("clientSecret is not configured.");
            }
            services.Configure<GoogleAuthSettings>(val =>
            {
                val.ClientId = clientId;
                val.ClientSecret = clientSecret;
            });
        
           
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




            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureDbContext(configuration);
            // Configure Redis connection
            // Add StackExchangeRedisCache as the IDistributedCache implementation
            
            // Add Mapper Services to Container injection
            services.AddAutoMapper(typeof(ApplicationMapper));

            services.AddAuthorization();


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                }).AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                    
                });

            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "F-Driver API", Version = "v1" });
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
            services.AddInfrastructureServices();

            return services;
        }
        private static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbTrustServerCertificate = Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE");
            var dbMultipleActiveResultSets = Environment.GetEnvironmentVariable("DB_MULTIPLE_ACTIVE_RESULT_SETS");
            //var connectionString = configuration.GetConnectionString("DefaultConnectionString");
            var connectionString = $"Data Source={dbServer};Initial Catalog={dbName};User ID={dbUser};Password={dbPassword};TrustServerCertificate={dbTrustServerCertificate};MultipleActiveResultSets={dbMultipleActiveResultSets}";

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
                .AddScoped<FirebaseService>()
                .AddScoped<EmailService>()
                .AddScoped<TripRequestService>()
                .AddScoped<CancellationReasonService>()
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
                .AddScoped<JwtSettings>()
                .AddScoped<GoogleAuthSettings>()
                .AddScoped<IdentityService>()










                ;



        }
    }
}
