using System.Reflection;
using Scalar.AspNetCore;
using Serilog;
using Template.API;
using Template.Application;
using Template.Application.Hubs;
using Template.Common.SharedKernel.Api.Cors;
using Template.Common.SharedKernel.Api.Endpoints;
using Template.Common.SharedKernel.Api.Middleware;
using Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework;
using Template.Infrastructure;
using Template.Infrastructure.Database;
using Template.ServiceDefaults;
using TickerQ.DependencyInjection.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.AddServiceDefaults();

builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.AddInfrastructure();
// builder.AddSeqEndpoint(Components.Seq);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

app.MapHub<RandomNumberHub>("random-number-hub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

// app.UseTickerQ();

await app.RunAsync();
