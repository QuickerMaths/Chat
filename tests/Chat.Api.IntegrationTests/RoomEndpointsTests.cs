using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Chat.Application.Contracts;

namespace Chat.Api.IntegrationTests;

[Collection(nameof(ChatApiCollection))]
public sealed class RoomEndpointsTests(ChatApiFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task CreateRoom_ShouldPersistAndReturnLocation_WhenAuthenticated()
    {
        var token = await fixture.RegisterAndLoginAsync("alice");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/rooms", new { name = "general" },
            cancellationToken: CancellationToken.None);
        var result = await response.Content.ReadFromJsonAsync<RoomDto>(CancellationToken.None);
        
        result!.Name.Should().Be("general");
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"/api/rooms/{result.Id}");
    }

    [Fact]
    public async Task CreateRoom_ShouldReturn409_WhenNameTaken()
    {
        var token = await fixture.RegisterAndLoginAsync("alice");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var creteResponse = await client.PostAsJsonAsync("/api/rooms", new { name = "general", },
            cancellationToken: CancellationToken.None);
        var room = await creteResponse.Content.ReadFromJsonAsync<RoomDto>(CancellationToken.None);

        var result = await client.PostAsJsonAsync("/api/rooms", new { name = "general", },
            cancellationToken: CancellationToken.None);

        room!.Name.Should().Be("general");
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ListRooms_ShouldFlagMembership_WhenCallerIsOwner()
    {
        var token = await fixture.RegisterAndLoginAsync("alice");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await client.PostAsJsonAsync("/api/rooms", new { name = "general" },
            cancellationToken: TestContext.Current.CancellationToken);
        var created = await createResponse.Content.ReadFromJsonAsync<RoomDto>(CancellationToken.None);

        var response = await client.GetAsync("/api/rooms", TestContext.Current.CancellationToken);
        var rooms = await response.Content.ReadFromJsonAsync<List<RoomListItemDto>>(CancellationToken.None);

        var room = rooms!.Should().ContainSingle(r => r.Id == created!.Id).Subject;
        room.IsMember.Should().BeTrue();
    }
}