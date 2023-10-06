using Videography.Application;
using Videography.Infrastructure;
using Videography.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

await app.UseWebApplication();

app.Run();
