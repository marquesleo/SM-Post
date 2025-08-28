using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventStore
{
    Task<List<BaseEvent>> GetEventAsync(Guid aggregateId);
    
    Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
    
}