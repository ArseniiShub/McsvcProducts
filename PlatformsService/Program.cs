global using PlatformsService.Models;
global using PlatformsService.Data;
global using Microsoft.EntityFrameworkCore;
using PlatformsService.AsyncDataServices;
using PlatformsService.SyncDataServices.Grpc;
using PlatformsService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var loggerFactory = LoggerFactory.Create(configure =>
{
	configure.ClearProviders();
	configure.AddConsole();
});
var logger = loggerFactory.CreateLogger<Program>();

if(builder.Environment.IsDevelopment())
{
	logger.LogInformation("Using In Memory Database");
	builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
}
else
{
	logger.LogInformation("Using Sql Server Database");
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
	);
}

builder.Services.AddGrpc();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddSingleton<PrepDb>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformsService>();
app.MapGet("/protos/platforms.proto",
	async context => await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto")));

var prep = app.Services.GetService<PrepDb>();
prep!.PrepPopulations(app, app.Environment.IsProduction());

app.Run();
