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
        var connectionString = configuration.GetConnectionString("ChatDb")
                               ?? throw new InvalidOperationException("Connection string 'ChatDb' is not configured.");

        services.AddScoped<PublishDomainInterceptor>();

        services.AddDbContext<ChatDbContext>((sp, options) =>
            options
                .UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<PublishDomainInterceptor>()));

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