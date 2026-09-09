using Chat.Application.Abstractions;
using Chat.Domain.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure.Persistence.Interceptors;

internal sealed class PublishDomainInterceptor(IServiceProvider serviceProvider) : SaveChangesInterceptor
{
    private readonly List<IDomainEvent> _pendingEvents = [];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default
    )
    {
        if (eventData.Context is { } context)
        {
            var entities = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count > 0)
                .ToList();

            foreach (var entity in entities)
            {
                _pendingEvents.AddRange(entity.DomainEvents);
                entity.ClearDomainEvents();
            }
        }
        
        return base.SavingChangesAsync(eventData, result, ct);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken ct = default)
    {
        var events = _pendingEvents.ToArray();
        _pendingEvents.Clear();

        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventsHandler<>).MakeGenericType(domainEvent.GetType());
            var method = handlerType.GetMethod(nameof(IDomainEventsHandler<IDomainEvent>.HandleAsync))!;

            foreach (var handler in serviceProvider.GetServices(handlerType))
            {
                if (handler is null)
                {
                    continue;
                }

                await (Task)method.Invoke(handler, [domainEvent, ct])!;
            }
        }
        
        return await base.SavedChangesAsync(eventData, result, ct);
    }
}