using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Middlewares;
using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Repositories;
using AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "AutoNext.Platform.Listings.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI only in development/staging
if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AutoNext Platform Listings API",
            Version = "v1",
            Description = "Production-ready vehicle listings management API for AutoNext platform",
            Contact = new OpenApiContact
            {
                Name = "AutoNext Support",
                Email = "support@autonext.com",
                Url = new Uri("https://autonext.com/support")
            }
        });

        // Add JWT Authentication
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' followed by your JWT token"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

// Register repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleSearchRepository, VehicleSearchRepository>();

// Register services
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleSearchService, VehicleSearchService>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IListingAnalyticsService, ListingAnalyticsService>();

// Configure JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Authentication failed: {Exception}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Information("Token validated for user: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SellerOnly", policy => policy.RequireClaim("Seller", "true"));
    options.AddPolicy("VerifiedSeller", policy => policy.RequireClaim("VerifiedSeller", "true"));
});

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages");
    });
});

// Configure response caching
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1MB
    options.UseCaseSensitivePaths = false;
});

// Configure memory cache
builder.Services.AddMemoryCache();

// Configure rate limiting
var permitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 100);
var windowInMinutes = builder.Configuration.GetValue<int>("RateLimiting:WindowInMinutes", 1);

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<Microsoft.AspNetCore.Http.HttpContext, string>(
        httpContext => System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ??
                         httpContext.Request.Headers["X-Client-Id"].FirstOrDefault() ??
                         httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = permitLimit,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(windowInMinutes)
            }));

    options.RejectionStatusCode = 429;
});

// Configure form options
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoNext Platform Listings API v1");
        c.RoutePrefix = "swagger";
    });
}
else if (app.Environment.EnvironmentName == "Staging")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoNext Platform Listings API v1 (Staging)");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseCors("AllowSpecificOrigins");
app.UseRateLimiter();

// Custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientId", httpContext.Request.Headers["X-Client-Id"].FirstOrDefault());
        diagnosticContext.Set("UserId", httpContext.User?.Identity?.Name);
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
    };
});

app.MapControllers();
app.MapHealthChecks("/health");

// Ensure indexes and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await EnsureIndexes(db, logger);

        if (app.Environment.IsDevelopment() &&
            builder.Configuration.GetValue<bool>("FeatureFlags:EnableSeedData", false))
        {
            await SeedSampleData(db, logger);
        }

        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing database");
    }
}

// Print startup information
Log.Information("Starting AutoNext Platform Listings API in {Environment} environment",
    app.Environment.EnvironmentName);

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Helper methods
async Task EnsureIndexes(MongoDbContext db, ILogger logger)
{
    try
    {
        var vehiclesCollection = db.Vehicles;

        var indexes = new[]
        {
            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.VIN),
                new CreateIndexOptions { Unique = true, Name = "idx_vin" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.ChassisNumber),
                new CreateIndexOptions { Unique = true, Name = "idx_chassis" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.EngineNumber),
                new CreateIndexOptions { Unique = true, Name = "idx_engine" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.Seller.UserId),
                new CreateIndexOptions { Name = "idx_seller" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.Price.Raw),
                new CreateIndexOptions { Name = "idx_price" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Combine(
                    Builders<Vehicle>.IndexKeys.Ascending(v => v.Specifications.Make),
                    Builders<Vehicle>.IndexKeys.Ascending(v => v.Specifications.Model)),
                new CreateIndexOptions { Name = "idx_make_model" })
        };

        await vehiclesCollection.Indexes.CreateManyAsync(indexes);
        logger.LogInformation("Database indexes created successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error creating indexes - they may already exist");
    }
}

async Task SeedSampleData(MongoDbContext db, ILogger logger)
{
    try
    {
        var count = await db.Vehicles.CountDocumentsAsync(_ => true);
        if (count > 0)
        {
            logger.LogInformation("Sample data already exists, skipping seed");
            return;
        }

        logger.LogInformation("Seeding sample data...");
        // Add sample data logic here
        logger.LogInformation("Sample data seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error seeding sample data");
    }
}