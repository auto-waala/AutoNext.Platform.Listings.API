using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Middlewares;
using AutoNext.Platform.Listings.API.Repositories;
using AutoNext.Platform.Listings.API.Models.Entities;
using AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURATION DEBUGGING ====================
Console.WriteLine("================================================");
Console.WriteLine("AutoNext Platform Listings API - Starting");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
Console.WriteLine("================================================");

// Debug configuration sources
var mongoDbConnectionString = builder.Configuration.GetSection("MongoDB:ConnectionString").Value;
var connectionStringsMongo = builder.Configuration.GetConnectionString("MongoDB");
var envVar = Environment.GetEnvironmentVariable("MongoDB__ConnectionString");

Console.WriteLine($"Config Source - MongoDB:ConnectionString: {(string.IsNullOrEmpty(mongoDbConnectionString) ? "NULL" : mongoDbConnectionString.Contains("__") ? "Has Placeholder!" : "Valid (value hidden)")}");
Console.WriteLine($"Config Source - ConnectionStrings:MongoDB: {(string.IsNullOrEmpty(connectionStringsMongo) ? "NULL" : connectionStringsMongo.Contains("__") ? "Has Placeholder!" : "Valid (value hidden)")}");
Console.WriteLine($"Environment Variable - MongoDB__ConnectionString: {(string.IsNullOrEmpty(envVar) ? "NOT SET" : "SET (value hidden)")}");
Console.WriteLine("================================================");

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

// Configure API Versioning - CORRECT SYNTAX
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-API-Version"),
        new UrlSegmentApiVersionReader()
    );
});

// Add API Explorer for versioning - NO PARAMETERS
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configure Swagger/OpenAPI - Enabled for ALL environments
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AutoNext Platform Listings API v1",
        Version = "v1",
        Description = "Vehicle listings management API for AutoNext platform - Version 1",
        Contact = new OpenApiContact
        {
            Name = "AutoNext Support",
            Email = "support@autonext.com",
            Url = new Uri("https://autonext.com/support")
        }
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "AutoNext Platform Listings API v2",
        Version = "v2",
        Description = "Vehicle listings management API for AutoNext platform - Version 2 (Enhanced)",
        Contact = new OpenApiContact
        {
            Name = "AutoNext Support",
            Email = "support@autonext.com",
            Url = new Uri("https://autonext.com/support")
        }
    });

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

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbContext>();

// Register Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INewlyArrivedRepository, NewlyArrivedRepository>();
builder.Services.AddScoped<IFeaturedVehicleRepository, FeaturedVehicleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IUsedVehiclesRepository, UsedVehiclesRepository>();
builder.Services.AddScoped<IPremiumVehicleRepository,PremiumVehicleRepository>();

// Register Services
builder.Services.AddScoped<INewlyArrivedService, NewlyArrivedService>();
builder.Services.AddScoped<IFeaturedVehicleService, FeaturedVehicleService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUsedVehiclesService, UsedVehiclesService>();
builder.Services.AddScoped<IPremiumVehicleService, PremiumVehicleService>();

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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                    ?? "YourSecretKeyHereAtLeast32CharactersLong!!!"))
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
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders(
                  "X-Total-Count", "X-Page", "X-Page-Size",
                  "X-Total-Pages", "X-API-Version");
    });
});

// Configure response caching
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1 MB
    options.UseCaseSensitivePaths = false;
});

// Configure memory cache
builder.Services.AddMemoryCache();

// Configure health checks
builder.Services.AddHealthChecks();

// Configure rate limiting
var permitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 100);
var windowInMinutes = builder.Configuration.GetValue<int>("RateLimiting:WindowInMinutes", 1);

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var partitionKey =
            httpContext.User.Identity?.Name ??
            httpContext.Request.Headers["X-Client-Id"].FirstOrDefault() ??
            httpContext.Connection.RemoteIpAddress?.ToString() ??
            "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
            new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = permitLimit,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(windowInMinutes)
            });
    });

    options.RejectionStatusCode = 429;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        var error = new
        {
            success = false,
            message = "Rate limit exceeded. Please try again later.",
            statusCode = 429,
            retryAfter = 60
        };

        await context.HttpContext.Response.WriteAsJsonAsync(error, cancellationToken);
    };
});

// Configure form options
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = 1024 * 1024; // 1 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHsts();
app.UseHttpsRedirection();

// Swagger enabled for ALL environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoNext Platform Listings API v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "AutoNext Platform Listings API v2");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "AutoNext Listings API Documentation";
    c.DisplayRequestDuration();
});

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
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        diagnosticContext.Set("ApiVersion", httpContext.GetRequestedApiVersion()?.ToString() ?? "Unknown");
    };
    options.GetLevel = (httpContext, _, ex) =>
    {
        if (ex is not null || httpContext.Response.StatusCode >= 500) return Serilog.Events.LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 400) return Serilog.Events.LogEventLevel.Warning;
        return Serilog.Events.LogEventLevel.Information;
    };
});

app.MapControllers();
app.MapHealthChecks("/health");

// Print startup URLs
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    var urls = app.Urls.Any() ? string.Join(", ", app.Urls) : "https://localhost:5001;http://localhost:5000";
    Console.WriteLine("=".PadRight(80, '='));
    Console.WriteLine("AutoNext Platform Listings API Started");
    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($"Listening on: {urls}");
    Console.WriteLine($"Swagger URL: {(app.Urls.FirstOrDefault() ?? "https://localhost:5001")}/swagger");
    Console.WriteLine("=".PadRight(80, '='));

    Log.Information("Swagger UI available at: {SwaggerUrl}/swagger", app.Urls.FirstOrDefault() ?? "https://localhost:5001");
}

// Ensure indexes and seed data
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await EnsureIndexes(db, logger);

        if (app.Environment.IsDevelopment() &&
            configuration.GetValue<bool>("FeatureFlags:EnableSeedData", false))
        {
            await SeedSampleData(db, logger);
        }

        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error initializing database");
    }
}

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
async Task EnsureIndexes(MongoDbContext db, ILogger<Program> logger)
{
    await Task.WhenAll(EnsureVehicleIndexes(db, logger), EnsureFeaturedVehicleIndexes(db, logger));

    logger.LogInformation("All database indexes created successfully");
}

async Task EnsureVehicleIndexes(MongoDbContext db, ILogger<Program> logger)
{
    try
    {
        var vehiclesCollection = db.Vehicles;

        var indexes = new[]
        {
            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.VIN),
                new CreateIndexOptions { Unique = true, Name = "idx_vin", Sparse = true }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.ChassisNumber),
                new CreateIndexOptions { Unique = true, Name = "idx_chassis", Sparse = true }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.EngineNumber),
                new CreateIndexOptions { Unique = true, Name = "idx_engine", Sparse = true }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.Seller.SellerId),
                new CreateIndexOptions { Name = "idx_seller" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.Price.Raw),
                new CreateIndexOptions { Name = "idx_price" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Combine(
                    Builders<Vehicle>.IndexKeys.Ascending(v => v.Specifications.Make),
                    Builders<Vehicle>.IndexKeys.Ascending(v => v.Specifications.Model)),
                new CreateIndexOptions { Name = "idx_make_model" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Descending(v => v.CreatedOn),
                new CreateIndexOptions { Name = "idx_created_on" }),

            new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Combine(
                    Builders<Vehicle>.IndexKeys.Ascending(v => v.IsActive),
                    Builders<Vehicle>.IndexKeys.Ascending(v => v.Status),
                    Builders<Vehicle>.IndexKeys.Descending(v => v.CreatedOn)),
                new CreateIndexOptions { Name = "idx_active_status_date" })
        };

        await vehiclesCollection.Indexes.CreateManyAsync(indexes);
        logger.LogInformation("Vehicle indexes created successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error creating Vehicle indexes - they may already exist");
    }
}

async Task EnsureFeaturedVehicleIndexes(MongoDbContext db, ILogger<Program> logger)
{
    try
    {
        var featuredVehiclesCollection = db.FeaturedVehicles;

        // Critical indexes (must have)
        var criticalIndexes = new[]
        {
            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Ascending(v => v.Slug),
                new CreateIndexOptions { Unique = true, Name = "idx_fv_slug" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Ascending(v => v.ModelSlug),
                new CreateIndexOptions { Unique = true, Name = "idx_fv_model_slug" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys
                    .Ascending(v => v.IsActive)
                    .Ascending(v => v.StartDate)
                    .Ascending(v => v.EndDate),
                new CreateIndexOptions { Name = "idx_fv_active_dates" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys
                    .Ascending(v => v.IsActive)
                    .Ascending(v => v.Priority)
                    .Descending(v => v.Rating),
                new CreateIndexOptions { Name = "idx_fv_active_priority_rating" })
        };

        await featuredVehiclesCollection.Indexes.CreateManyAsync(criticalIndexes);
        logger.LogInformation("FeaturedVehicle critical indexes created successfully");

        // Optional indexes (can be created in background)
        var optionalIndexes = new[]
        {
            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Ascending(v => v.BrandName),
                new CreateIndexOptions { Name = "idx_fv_brand_name" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Ascending(v => v.VehicleType),
                new CreateIndexOptions { Name = "idx_fv_vehicle_type" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Ascending("Price.Amount"),
                new CreateIndexOptions { Name = "idx_fv_price_amount" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Ascending("Location.City"),
                new CreateIndexOptions { Name = "idx_fv_location_city" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys
                    .Text(v => v.Title)
                    .Text(v => v.Descriptions)
                    .Text(v => v.BrandName)
                    .Text(v => v.ModelName),
                new CreateIndexOptions { Name = "idx_fv_search_text" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Descending(v => v.CreatedAt),
                new CreateIndexOptions { Name = "idx_fv_created_at" }),

            new CreateIndexModel<FeaturedVehicle>(
                Builders<FeaturedVehicle>.IndexKeys.Descending("Engagement.Views"),
                new CreateIndexOptions { Name = "idx_fv_engagement_views" })
        };

        await featuredVehiclesCollection.Indexes.CreateManyAsync(optionalIndexes);
        logger.LogInformation("FeaturedVehicle optional indexes created successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error creating FeaturedVehicle indexes - they may already exist");
    }
}



async Task SeedSampleData(MongoDbContext db, ILogger<Program> logger)
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

        var sampleVehicle = new Vehicle
        {
            Title = "Sample Honda City 2020",
            Description = "Well-maintained Honda City, single owner, excellent condition",
            Make = "Honda",
            Model = "City",
            Year = 2020,
            Price = 850000,
            Kilometers = 45000,
            FuelType = "Petrol",
            Transmission = "Automatic",
            Color = "White",
            City = "Hyderabad",
            Images = new List<string>(),
            SellerId = "sample_seller_001",
            SellerName = "John Doe",
            SellerPhone = "+919876543210",
            CreatedBy = "system",
            CreatedOn = DateTime.UtcNow,
            ModifiedBy = "system",
            ModifiedOn = DateTime.UtcNow,
            IsActive = true,
            Status = "active",
            Views = 0
        };

        await db.Vehicles.InsertOneAsync(sampleVehicle);
        logger.LogInformation("Sample data seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error seeding sample data");
    }
}