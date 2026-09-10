using System.Net;
using System.Net.Http.Json;
using Chat.Application.Contracts;

namespace Chat.Api.IntegrationTests;

[Collection(nameof(ChatApiCollection))]
public sealed class AuthEndPointsTests(ChatApiFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Register_ShouldReturn201_WhenUserNameIsFree()
    {
        var client = fixture.CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register",
         new { userName = "alice",password = "Passw0rd!" }, cancellationToken: CancellationToken.None);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_ShouldReturnJwt_WhenCredentialsValid()
    {
        var client = fixture.CreateClient();
        await client.PostAsJsonAsync("api/auth/register",
            new { userName = "alice", password = "Passw0rd!"}, cancellationToken: CancellationToken.None);
        
        
        var response = await client.PostAsJsonAsync("api/auth/login",
            new { userName = "alice", password = "Passw0rd!"}, cancellationToken: CancellationToken.None);
        var token = await response.Content.ReadFromJsonAsync<AccessTokenDto>(CancellationToken.None);

        token!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task Login_ShouldReturn400_WhenCredentialsInvalid()
    {
        var client = fixture.CreateClient();
        await client.PostAsJsonAsync("api/auth/register",
            new { userName = "alice", password = "Passw0rd!"}, cancellationToken: CancellationToken.None);
        
        var response = await client.PostAsJsonAsync("api/auth/login",
            new { userName = "alice", password = "wrongPassw0rd!"}, cancellationToken: CancellationToken.None);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRooms_ShouldReturn401_WhenNoToken()
    {
        var client = fixture.CreateClient();
        
        var response = await client.GetAsync("api/rooms", cancellationToken: CancellationToken.None);
        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}