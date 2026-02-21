using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Infrastructure.Persistence;
using TaskTracker.Infrastructure.Services;

namespace TaskTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Try to get connection string from 'CONNETION_STRING' (User specific typo) or 'CONNECTION_STRING'
        var connectionString = Environment.GetEnvironmentVariable("CONNETION_STRING");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        // 2. Try 'DATABASE_URL' environment variable (common in Render/Heroku)
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
        }

        // 3. If not found, try standard configuration (appsettings.json or ConnectionStrings__DefaultConnection env var)
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Parse Render/Heroku style connection URL (postgres://...) if needed
        if (!string.IsNullOrEmpty(connectionString) && (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://")))
        {
            try 
            {
                var databaseUri = new Uri(connectionString);
                var userInfo = databaseUri.UserInfo.Split(':');
                var builder = new Npgsql.NpgsqlConnectionStringBuilder
                {
                    Host = databaseUri.Host,
                    Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = databaseUri.LocalPath.TrimStart('/'),
                    SslMode = Npgsql.SslMode.Require,
                    TrustServerCertificate = true // Needed for some cloud providers
                };
                connectionString = builder.ToString();
            }
            catch (Exception ex)
            {
                // Fallback or log if parsing fails, though in startup we might just let it crash
                Console.WriteLine($"Error parsing Connection String URI: {ex.Message}");
            }
        }

        // Diagnostic Log (Masked Password)
        Console.WriteLine($"[Startup] Using Connection String: {(string.IsNullOrEmpty(connectionString) ? "NULL" : "Present (Password Hidden)")}");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddTransient<IDateTimeService, DateTimeService>();
        
        return services;
    }
}
