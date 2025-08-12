using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;
using Serilog;
using Template.API;
using Template.API.Cors;
using Template.API.Extensions;
using Template.Application;
using Template.Infrastructure;
using Template.ServiceDefaults;
using TickerQ.DependencyInjection.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.AddServiceDefaults();

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

app.MapGet("/api/weatherforecast", () => Results.Ok("Hello World!"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));
    app.ApplyMigrations();
}

app.UseTickerQ();

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

await app.RunAsync();
