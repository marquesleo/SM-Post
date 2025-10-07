using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.InfraEstructure;
using CQRS.Core.Producers;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using Post.Common.Events;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Registra o serializer para Guid
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));



var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
var dispacher = new CommandDispatcher();

dispacher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
dispacher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
dispacher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
dispacher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
dispacher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
dispacher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
dispacher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);


builder.Services.AddSingleton<ICommandDispatcher>(_ => dispacher);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("documentation");
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

