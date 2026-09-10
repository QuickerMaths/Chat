using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Chat.Application.Contracts;

namespace Chat.Api.IntegrationTests;

[Collection(nameof(ChatApiCollection))]
public sealed class MessageEndpointsTests(ChatApiFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task SendMessage_ShouldReturn403_WhenCallerNotMember()
    {
        var ownerToken = await fixture.RegisterAndLoginAsync("alice");
        var notMemberToken = await fixture.RegisterAndLoginAsync("bob");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);

        var createResponse = await client.PostAsJsonAsync("/api/rooms", new { name = "general" },
            cancellationToken: TestContext.Current.CancellationToken);
        var room = await createResponse.Content.ReadFromJsonAsync<RoomDto>(TestContext.Current.CancellationToken);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", notMemberToken);

        var response = await client.PostAsJsonAsync($"/api/rooms/{room!.Id}/messages",
            new { body = "Hello, World!", clientMessageId = Guid.NewGuid() },
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(TestContext.Current.CancellationToken);
        problem!.Code.Should().Be("room.not_member");
    }
    
    [Fact]
    public async Task SendMessage_ShouldReturnSameMessage_WhenClientMessageIdRepeated()
    {
        var clientMessageId = Guid.NewGuid();
        var token = await fixture.RegisterAndLoginAsync("alice");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var createResponse = await client.PostAsJsonAsync("/api/rooms", new { name = "general" },
            cancellationToken: TestContext.Current.CancellationToken);
        var room = await createResponse.Content.ReadFromJsonAsync<RoomDto>(TestContext.Current.CancellationToken);
        
        var firstCall = await client.PostAsJsonAsync($"/api/rooms/{room!.Id}/messages",
            new { body = "Hello, World!", clientMessageId },
            cancellationToken: TestContext.Current.CancellationToken);
        var secondCall = await client.PostAsJsonAsync($"/api/rooms/{room!.Id}/messages",
            new { body = "Hello, World!", clientMessageId },
            cancellationToken: TestContext.Current.CancellationToken);
        var firstResponse = await firstCall.Content.ReadFromJsonAsync<MessageDto>(TestContext.Current.CancellationToken);
        var secondResponse = await secondCall.Content.ReadFromJsonAsync<MessageDto>(TestContext.Current.CancellationToken);

        firstResponse!.Id.Should().Be(secondResponse!.Id);

        var listResponse = await client.GetAsync($"/api/rooms/{room.Id}/messages", TestContext.Current.CancellationToken);
        var messages = await listResponse.Content.ReadFromJsonAsync<List<MessageDto>>(TestContext.Current.CancellationToken);

        messages.Should().ContainSingle(m => m.Id == firstResponse.Id);
    }
    
    [Fact]
    public async Task GetMessages_ShouldPageByBeforeCursor_WhenMoreThanLimit()
    {
        var token = await fixture.RegisterAndLoginAsync("alice");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResponse = await client.PostAsJsonAsync("/api/rooms", new { name = "general" },
            cancellationToken: TestContext.Current.CancellationToken);
        var room = await createResponse.Content.ReadFromJsonAsync<RoomDto>(TestContext.Current.CancellationToken);

        foreach (var body in new[] { "first", "second", "third" })
        {
            await client.PostAsJsonAsync($"/api/rooms/{room!.Id}/messages",
                new { body, clientMessageId = Guid.NewGuid() },
                cancellationToken: TestContext.Current.CancellationToken);
        }

        var firstPageResponse = await client.GetAsync($"/api/rooms/{room!.Id}/messages?limit=2",
            TestContext.Current.CancellationToken);
        var firstPage = await firstPageResponse.Content.ReadFromJsonAsync<List<MessageDto>>(TestContext.Current.CancellationToken);

        firstPage!.Select(m => m.Body).Should().Equal("second", "third");

        var oldestOfFirstPage = firstPage![0];
        var before = Uri.EscapeDataString(oldestOfFirstPage.SentAtUtc.ToString("O"));

        var secondPageResponse = await client.GetAsync(
            $"/api/rooms/{room!.Id}/messages?limit=2&before={before}",
            TestContext.Current.CancellationToken);
        var secondPage = await secondPageResponse.Content.ReadFromJsonAsync<List<MessageDto>>(TestContext.Current.CancellationToken);

        secondPage!.Should().ContainSingle();
        secondPage[0].Body.Should().Be("first");
    }

    private sealed record ProblemResponse(string? Code);
}