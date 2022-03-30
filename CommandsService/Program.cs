global using CommandsService.Models;
global using CommandsService.Data;
global using Microsoft.EntityFrameworkCore;
using CommandsService.AsyncDataServices;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton<PrepDb>();

builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

builder.Services.AddHostedService<MessageBusSubscriber>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var prep = app.Services.GetService<PrepDb>();
prep!.PrepPopulation(app);

app.Run();
