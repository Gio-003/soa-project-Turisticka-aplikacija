using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using tour_service.Data;
using tour_service.Repositories;
using tour_service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TourRepository>();
builder.Services.AddScoped<KeyPointRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<KeyPointService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<ReviewService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Automatski primijeni migracije
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();