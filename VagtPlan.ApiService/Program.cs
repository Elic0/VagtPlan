using ApiService.Controllers;
using ApiService.DBContext;
using ApiService.Services;
using ApiService.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System;
using System.Reflection;
using System.Text;

namespace ApiService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        IConfiguration Configuration = builder.Configuration;

        string? connectionString = Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(connectionString));

        // Add services to the container.

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        builder.Services.AddControllers();

        builder.Services.AddSignalR();

        // Used for SignalR
        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });

        // JWT Authentication setup
        // Registrer JWT Service
        builder.Services.AddScoped<JwtService>();

        // Konfigurer JWT Authentication
        string? jwtSecretKey = Configuration["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(jwtSecretKey))
        {
            jwtSecretKey = Environment.GetEnvironmentVariable("Jwt__SecretKey");
        }
        if (string.IsNullOrWhiteSpace(jwtSecretKey))
        {
            throw new InvalidOperationException("JWT secret key not configured. Set 'Jwt__SecretKey' environment variable or 'Jwt:SecretKey' in configuration.");
        }

        string? jwtIssuer = Configuration["Jwt:Issuer"];
        if (string.IsNullOrWhiteSpace(jwtIssuer))
        {
            jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer");
        }
        if (string.IsNullOrWhiteSpace(jwtIssuer))
        {
            throw new InvalidOperationException("JWT issuer not configured. Set 'Jwt__Issuer' environment variable or 'Jwt:Issuer' in configuration.");
        }

        string? jwtAudience = Configuration["Jwt:Audience"];
        if (string.IsNullOrWhiteSpace(jwtAudience))
        {
            jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience");
        }
        if (string.IsNullOrWhiteSpace(jwtAudience))
        {
            throw new InvalidOperationException("JWT audience not configured. Set 'Jwt__Audience' environment variable or 'Jwt:Audience' in configuration.");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        // Add CORS support for Flutter app
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFlutterApp", policy =>
            {
                policy.WithOrigins(
                        "https://vagtplan.dk",
                        "https://vagtplan.dk"
                    )
                    .AllowAnyMethod()               // Allow GET, POST, PUT, DELETE, etc.
                    .AllowAnyHeader()               // Allow any headers
                    .AllowCredentials();            // Allow cookies/auth headers
            });

            // Development policy - more permissive for local development
            options.AddPolicy("AllowAllLocalhost", policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    // Tillad alle localhost og 127.0.0.1 origins med alle porte
                    var uri = new Uri(origin);
                    return uri.Host == "localhost" ||
                           uri.Host == "127.0.0.1" ||
                           uri.Host == "0.0.0.0";
                })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // Use the new .NET 10 OpenAPI
        builder.Services.AddOpenApi();

        var app = builder.Build();

        var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
        RunDatabaseStartup(app, startupLogger).GetAwaiter().GetResult();

        // Used for SignalR
        app.UseResponseCompression();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.

        app.UseForwardedHeaders();

        app.MapOpenApi();

        // SignalR Hub endpoint

        // Enable Swagger UI (klassisk dokumentation (Med Darkmode))
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "API v1");
            options.RoutePrefix = "swagger"; // Tilgængelig på /swagger
        });

        app.UseStaticFiles();


        // Enable Scalar UI (moderne alternativ til Swagger UI)
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("API Documentation")
                   .WithTheme(ScalarTheme.Purple)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });


        // Enable CORS - SKAL være før UseAuthorization
        app.UseCors(app.Environment.IsDevelopment() ? "AllowAllLocalhost" : "AllowFlutterApp");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Log API dokumentations URL'er ved opstart
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
                .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;

            if (addresses != null && app.Environment.IsDevelopment())
            {
                foreach (var address in addresses)
                {
                    logger.LogInformation("Swagger UI: {Address}/swagger", address);
                    logger.LogInformation("Scalar UI:  {Address}/scalar", address);
                }
            }
        });

        app.Run();
    }

    private static async Task RunDatabaseStartup(WebApplication app, ILogger logger)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

        await WaitForDatabaseAsync(db, logger);

        db.Database.Migrate();
        await AdminUserSeeder.SeedAsync(db, app.Configuration);
    }

    private static async Task WaitForDatabaseAsync(
        AppDBContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(1);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (await db.Database.CanConnectAsync(cancellationToken))
                {
                    if (attempt > 1)
                    {
                        logger.LogInformation(
                            "Database connection established on attempt {Attempt}.",
                            attempt);
                    }

                    return;
                }
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex,
                    "Database was not ready on attempt {Attempt}/{MaxAttempts}. Retrying in {DelaySeconds}s.",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(delay, cancellationToken);
            }
        }

        throw new InvalidOperationException(
            $"Database did not become available within {maxAttempts * delay.TotalSeconds} seconds.");
    }
}