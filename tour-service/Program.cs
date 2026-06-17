using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using tour_service.Clients;
using tour_service.Data;
using tour_service.Repositories;
using tour_service.Services;
using tour_service.Saga;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<PurchaseRpcClient>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("tour-service"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TourRepository>();
builder.Services.AddScoped<KeyPointRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<KeyPointService>();
builder.Services.AddScoped<TourDomainService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<TourDurationRepository>();
builder.Services.AddScoped<TourDurationService>();
builder.Services.AddScoped<TourExecutionRepository>();
builder.Services.AddScoped<TourExecutionService>();
builder.Services.AddSingleton<PublishTourOrchestrator>();
builder.Services.AddHostedService<RegistrationTourSagaListener>();
builder.Services.AddGrpc();
builder.Services.AddCors(options => //dodato odavde 

{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
}); //do ovde

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });

    options.ListenAnyIP(9090, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); // dodato

app.UseCors("AllowAngular"); // dodato

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<TourGrpcService>();
// Automatski primijeni migracije
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
