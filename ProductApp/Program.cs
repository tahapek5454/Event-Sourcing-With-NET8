using Shared.Services.Abstract;
using Shared.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var eventSourceConnectionString = builder.Configuration["EventSource"];

builder.Services.AddSingleton<IEventStoreService>(_ => new EventStoreService(eventSourceConnectionString ?? string.Empty));


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
