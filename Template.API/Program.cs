using Hangfire;
using Scalar.AspNetCore;
using Serilog;
using Template.API;
using Template.API.Cors;
using Template.API.Extensions;
using Template.Application;
using Template.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.AddServiceDefaults();

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseBackgroundJobs();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));

    app.ApplyMigrations();

    app.UseHangfireDashboard(options: new DashboardOptions
    {
        Authorization = [],
        DarkModeEnabled = true
    });
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
