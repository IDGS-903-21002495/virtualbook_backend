using Microsoft.EntityFrameworkCore;
using Sentry;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using virtualbook_backend.Data;
var builder = WebApplication.CreateBuilder(args);

// Configure Sentry
var sentryDsn = builder.Configuration["SENTRY_DSN_BACKEND"];
if (!string.IsNullOrEmpty(sentryDsn))
{
    builder.WebHost.UseSentry(o =>
    {
        o.Dsn = sentryDsn;
        o.Debug = true;

        o.TracesSampleRate = 1.0;
        o.Environment = builder.Environment.EnvironmentName;
        o.Release = "virtualbook-backend@1.0.0";
        o.Debug = builder.Environment.IsDevelopment();
    });

    builder.Logging.AddSentry(o =>
    {
        o.Dsn = sentryDsn;
        
    });
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Configurar rutas en minúsculas
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VirtualBookDbContext>(options =>
    options.UseNpgsql(connectionString)
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

if (!string.IsNullOrEmpty(sentryDsn))
{
    app.UseSentryTracing();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
