using Chat.Application.Abstractions;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Persistence.Interceptors;
using Chat.Infrastructure.Persistence.Queries;
using Chat.Infrastructure.Persistence.Repositories;
using Chat.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<PublishDomainInterceptor>();

        services.AddDbContext<ChatDbContext>((sp, options) =>
        {
            // Read the connection string lazily, from the service provider's own
            // IConfiguration, rather than the 'configuration' parameter captured at
            // AddInfrastructure() call time. WebApplicationFactory<T> only splices its
            // ConfigureAppConfiguration overrides in at builder.Build() time, which runs
            // after Program.cs calls AddInfrastructure() — an eagerly captured value would
            // never see those overrides (e.g. Testcontainers connection strings in tests).
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("ChatDb")
                                   ?? throw new InvalidOperationException("Connection string 'ChatDb' is not configured.");

            options
                .UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<PublishDomainInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ChatDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddScoped<IRoomQueries, RoomQueries>();
        services.AddScoped<IMessageQueries, MessageQueries>();
        services.AddScoped<IUserQueries, UserQueries>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ITokenIssuer, JwtTokenIssuer>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration("Jwt")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}