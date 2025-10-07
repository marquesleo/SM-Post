using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Post.Cmd.Infrastructure.Config;
using Post.Common.Events;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository  :IEventStoreRepository
{
    
    private readonly IMongoCollection<EventModel>  _eventStoreCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEvent)))
        {
            BsonClassMap.RegisterClassMap<BaseEvent>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.AddKnownType(typeof(PostCreatedEvent)); // seu evento concreto
            });

            /*BsonClassMap.RegisterClassMap<PostCreatedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BaseEvent>();
            BsonClassMap.RegisterClassMap<PostCreatedEvent>();
            BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
            BsonClassMap.RegisterClassMap<PostLikedEvent>();
            BsonClassMap.RegisterClassMap<CommentAddedEvent>();
            BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
            BsonClassMap.RegisterClassMap<PostRemovedEvent>();*/
        }

        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        
    }
    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);

    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        try
        {
            return await _eventStoreCollection.FindSync(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }
}