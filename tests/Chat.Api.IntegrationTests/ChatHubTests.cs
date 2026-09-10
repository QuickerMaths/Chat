using System.Net.Http.Headers;
using System.Net.Http.Json;
using Chat.Application.Contracts;

namespace Chat.Api.IntegrationTests;

[Collection(nameof(ChatApiCollection))]
public sealed class ChatHubTests(ChatApiFixture fixture) : IAsyncLifetime
{
    public ValueTask InitializeAsync() => new(fixture.ResetAsync());
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Hub_ShouldPushMessageToRoomGroup_WhenMemberPosts()
    {
        var token = await fixture.RegisterAndLoginAsync("alice");
        var client = fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createResponse = await client.PostAsJsonAsync("/api/rooms", new { name = "general" },
            TestContext.Current.CancellationToken);
        var room = await createResponse.Content.ReadFromJsonAsync<RoomDto>(CancellationToken.None);
        var roomId = room!.Id;

        var recevied = new TaskCompletionSource<MessageDto>();

        await using var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(client.BaseAddress!, "/hubs/chat"), o =>
            {
                o.HttpMessageHandlerFactory = _ => fixture.Server.CreateHandler();
                o.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .Build();
        
        connection.On<MessageDto>("MessageReceived", dto => recevied.TrySetResult(dto));
        await connection.StartAsync(TestContext.Current.CancellationToken);
        await connection.InvokeAsync("JoinRoom", roomId, cancellationToken: CancellationToken.None);
        
        await client.PostAsJsonAsync($"/api/rooms/{roomId}/messages", 
                new { body = "hello", clientMessageId = Guid.NewGuid() }, cancellationToken: CancellationToken.None);
        
        var pushed =  await recevied.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken: CancellationToken.None);
        pushed.Body.Should().Be("hello");
    }
}