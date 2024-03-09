using ProductEventHandlerService;
using Shared.Services.Abstract;
using Shared.Services.Concrete;

var builder = Host.CreateApplicationBuilder(args);

var eventSourceConnectionString = builder.Configuration["EventSource"];

builder.Services.AddSingleton<IEventStoreService>(_ => new EventStoreService(eventSourceConnectionString ?? string.Empty));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
