using AISA.Domain.Interfaces;
using AISA.Infrastructure.ExternalServices;
using AISA.Infrastructure.Persistence;
using AISA.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AISA.Infrastructure;

/// <summary>
/// Extensie pentru înregistrarea serviciilor Infrastructure Layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL + EF Core
        services.AddDbContext<AisaDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AisaDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IBusinessProfileRepository, BusinessProfileRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // JWT Service
        services.AddSingleton<AISA.Application.Common.Interfaces.IJwtService, AISA.Infrastructure.Auth.JwtService>();

        // AI Sentiment HTTP Client
        services.AddHttpClient<ISentimentAnalysisService, AiSentimentClient>(client =>
        {
            var aiServiceUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
            client.BaseAddress = new Uri(aiServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Review Scraper HTTP Client
        services.AddHttpClient<IReviewScraperClient, ReviewScraperClient>(client =>
        {
            var aiServiceUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
            client.BaseAddress = new Uri(aiServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(60); // scraping poate dura mai mult
        });

        return services;
    }
}
