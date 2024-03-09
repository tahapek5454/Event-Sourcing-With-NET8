using ProductEventHandlerService.Services;
using Shared.Services.Abstract;
using Shared.Services.Concrete;

var builder = Host.CreateApplicationBuilder(args);

var eventSourceConnectionString = builder.Configuration["EventSource"];

var mongoDbConnectionString = builder.Configuration["MongoDb:ConnectionString"];
var mongoDbName= builder.Configuration["MongoDb:DataBaseName"];

builder.Services.AddSingleton<IEventStoreService>(_ => new EventStoreService(eventSourceConnectionString ?? string.Empty));

builder.Services.AddSingleton<IMongoDbService>(_ => new MongoDbService(mongoDbConnectionString, mongoDbName));

builder.Services.AddHostedService<EventStoreServiceBG>();

var host = builder.Build();
host.Run();
