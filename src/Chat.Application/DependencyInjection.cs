using Chat.Application.Abstractions;
using Chat.Application.Features.Auth.LoginUser;
using Chat.Application.Features.Auth.RegisterUser;
using Chat.Application.Features.Messages.GetMessages;
using Chat.Application.Features.Messages.SendMessage;
using Chat.Application.Features.Rooms.CreateRoom;
using Chat.Application.Features.Rooms.JoinRoom;
using Chat.Application.Features.Rooms.ListRooms;
using Chat.Domain.Messages.Events;

namespace Chat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginUserHandler>();
        services.AddScoped<CreateRoomHandler>();
        services.AddScoped<JoinRoomHandler>();
        services.AddScoped<ListRoomsHandler>();
        services.AddScoped<SendMessageHandler>();
        services.AddScoped<SendMessageHandler>();
        services.AddScoped<GetMessagesHandler>();

        services.AddScoped<IDomainEventsHandler<MessageSent>>();

        services.AddScoped<IValidator<RegisterUserCommand>>();
        services.AddScoped<IValidator<LoginUserCommand>>();
        services.AddScoped<IValidator<CreateRoomCommand>>();
        
        return services;
    }
}