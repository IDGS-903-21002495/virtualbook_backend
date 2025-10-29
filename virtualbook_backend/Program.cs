using Microsoft.EntityFrameworkCore;
using Sentry;
using virtualbook_backend.Data;
var builder = WebApplication.CreateBuilder(args);

// Sentry Initialization
builder.WebHost.UseSentry(o =>
{
    o.Dns = "SENTRY_DSN_BACKEND";
    o.Debug = true;

    // Configuraciones adicionales de Sentry
    o.TracesSampleRate = 1.0; 
    o.Environment = builder.Environment.EnvironmentName;
    o.Release = "virtualbook-backend@1.0.0";
    o.Debug = builder.Environment.IsDevelopment();
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VirtualBookDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSentryTracing();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
