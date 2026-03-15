using RoadmapCreationAssistance.API.Extensions;
using RoadmapCreationAssistance.API.Middlewares;
using Serilog;
using Serilog.Sinks.Datadog.Logs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.Enrich.FromLogContext()
      .WriteTo.Console();

    string? ddApiKey = Environment.GetEnvironmentVariable("DD_API_KEY");

    if (!string.IsNullOrWhiteSpace(ddApiKey))
    {
        DatadogConfiguration datadogConfig = new DatadogConfiguration
        {
            Url = "https://http-intake.logs.us3.datadoghq.com",
            Port = 443,
            UseSSL = true,
            UseTCP = false
        };
        lc.WriteTo.DatadogLogs(
            ddApiKey,
            configuration: datadogConfig,
            service: Environment.GetEnvironmentVariable("DD_SERVICE") ?? "roadmap-creation-assistance-api",
            host: Environment.MachineName);
    }
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRoadmapServices(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["GitHubApi:HealthCheckUrl"]!), "github")
    .AddUrlGroup(new Uri(builder.Configuration["OpenAIApi:HealthCheckUrl"]!), "openai");

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

await app.RunAsync();