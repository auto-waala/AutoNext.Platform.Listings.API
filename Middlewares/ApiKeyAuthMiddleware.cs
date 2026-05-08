using AutoNext.Platform.Listings.API.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace AutoNext.Platform.Listings.API.Middlewares
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.ForContext<ApiKeyAuthMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, MongoDbContext dbContext)
        {
            // Skip auth for swagger and health checks
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Client-Id", out var clientId) ||
                !context.Request.Headers.TryGetValue("X-Client-Secret", out var clientSecret))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing API credentials");
                return;
            }

            var client = await dbContext.Clients
                .Find(c => c.ClientId == clientId && c.IsActive)
                .FirstOrDefaultAsync();

            if (client == null || !BCrypt.Net.BCrypt.Verify(clientSecret, client.ClientSecret))
            {
                _logger.Warning("Invalid API credentials for ClientId: {ClientId}", clientId);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API credentials");
                return;
            }

            await _next(context);
        }
    }
}
