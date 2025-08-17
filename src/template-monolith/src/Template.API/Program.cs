using System.Reflection;
using Scalar.AspNetCore;
using Serilog;
using Template.API;
using Template.API.Cors;
using Template.Application;
using Template.Common.SharedKernel.Api.Endpoints;
using Template.Common.SharedKernel.Infrastructure.EF;
using Template.Infrastructure;
using Template.Infrastructure.Database;
using Template.ServiceDefaults;
using TickerQ.DependencyInjection.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.AddServiceDefaults();

builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddApplication();
builder.AddInfrastructure();
// builder.AddSeqEndpoint(Components.Seq);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

// app.MapHub<RandomNumberHub>("random-number-hub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));
    app.ApplyMigrations<ApplicationDbContext>();
}

app.UseTickerQ();

app.UseHttpsRedirection();

// app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

await app.RunAsync();
