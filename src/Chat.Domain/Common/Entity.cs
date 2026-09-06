namespace Chat.Domain.Common;

public abstract class Entity<TId> where TId : struct
{
    private readonly List<IDomainEvent> _domainEvents = [];
    
    public TId Id { get; protected init; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}