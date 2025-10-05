using System.Data;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{

    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository,
                      IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task<List<BaseEvent>> GetEventAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException("Incorrect aggregate id");
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();

    }

    public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {

        try
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException();

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                @event.Type = @event.GetType().Name;
            //    @event.Id = Guid.NewGuid();
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel()
                {
                    TimeStamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventData = @event,
                    EventType = eventType,
                    Id = Guid.NewGuid().ToString(),

                };

                await _eventStoreRepository.SaveAsync(eventModel);
                var topic = "SocialMediaPostEvents";
                await _eventProducer.ProducerAsync(topic, @event);
            }
         }
        catch (Exception ex)
        {

            throw;
        }



    }
}