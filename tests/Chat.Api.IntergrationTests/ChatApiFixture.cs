using System.Net.Http.Json;
using Chat.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Api.IntergrationTests;

public sealed class ChatApiFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-CU6-ubuntu-24.04")
        .WithPassword("Your_strong_Passw0rd!")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private Respawner _respawner = null!;
    private string _connectionString = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _connectionString,
                })));

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            await db.Database.MigrateAsync();
        }
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection,
            new RespawnerOptions { DbAdapter = DbAdapter.SqlServer, TablesToIgnore = ["_EFMigrationsHistory"], });
    }
    
    public HttpClient CreateClient() => _factory.CreateClient();

    public async Task ResetAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async Task<string> RegisterAndLoginAsync(string userName)
    {
        var client = CreateClient();
        var credentials = new { userName, password = "Passw0rd!23" };

        await client.PostAsJsonAsync("/auth/register", credentials);
        var response = await client.PostAsJsonAsync("/auth/login", credentials);
        
        var token = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        return token!.AccessToken;
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _container.DisposeAsync();
    }

    private sealed record AccessTokenResponse(
        string AccessToken,
        DateTimeOffset ExpiresAtUtc,
        string UserName,
        Guid UserId);
}