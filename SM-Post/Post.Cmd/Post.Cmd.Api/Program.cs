using Confluent.Kafka;
using CQRS.Core.Domain;
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

var builder = WebApplication.CreateBuilder(args);

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
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
