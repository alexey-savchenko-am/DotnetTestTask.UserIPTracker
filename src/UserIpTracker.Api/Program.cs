using MinimalApi.Endpoint.Extensions;
using UserIpTracker.Application;
using UserIpTracker.Infrastructure;
using UserIpTracker.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddApplication()
    .AddInfrastructure();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

services.AddEndpoints();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var scope = app.Services.CreateScope();
    var databaseInitializer = scope.ServiceProvider.GetService<IDatabaseInitializer>();
    databaseInitializer?.InitializeAsync(recreateDatabase: true).Wait();
    scope.Dispose();
}

app.UseCors();

app.UseHttpsRedirection();

app.Run();
