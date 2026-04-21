using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Data;
using QLNhanVien.src.Data.Repositories;
using QLNhanVien.src.Models.Entities;
using QLNhanVien.src.Services;
using DotNetEnv;

// Load environment variables từ .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ================== ENVIRONMENT & CONFIGURATION ==================
var environment = builder.Environment.EnvironmentName;
var logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Information";

// Configure Logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
    config.SetMinimumLevel(Enum.Parse<LogLevel>(logLevel));
});

var logger = LoggerFactory.Create(config => config.AddConsole())
    .CreateLogger("Startup");

logger.LogInformation("========================================");
logger.LogInformation("🚀 Starting QLNhanVien Application");
logger.LogInformation("Environment: {Environment}", environment);
logger.LogInformation("Log Level: {LogLevel}", logLevel);
logger.LogInformation("========================================");

// ================== DATABASE CONFIGURATION ==================
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")?.Trim().Trim('"');

if (string.IsNullOrWhiteSpace(connectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";

    if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
    {
        logger.LogError("❌ Database configuration incomplete. Check .env file.");
        throw new InvalidOperationException("Missing required database configuration in .env file");
    }

    connectionString = $"Server={dbHost};Port={dbPort};User Id={dbUser};Password={dbPassword};Database={dbName};SSL Mode=Require;";
    logger.LogInformation("✓ Built connection string from individual environment variables");
}
else
{
    logger.LogInformation("✓ Using DATABASE_URL from environment");
}

logger.LogInformation("📡 Database Host: {Host}", GetHostFromConnectionString(connectionString));

var migrateOnStartup = bool.TryParse(Environment.GetEnvironmentVariable("MIGRATE_ON_STARTUP"), out var migrateFlag)
    ? migrateFlag
    : true;

// Add DbContext
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Command timeout
        npgsqlOptions.CommandTimeout(
            int.TryParse(Environment.GetEnvironmentVariable("EF_POOLING_TIMEOUT_SECONDS"), out var timeout)
                ? timeout
                : 30
        );

        // Connection pooling
        var poolSize = int.TryParse(Environment.GetEnvironmentVariable("EF_POOL_SIZE"), out var size) ? size : 10;

        // Enable retry on failure
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);

        logger.LogInformation("✓ Npgsql configured - CommandTimeout: {Timeout}s, PoolSize: {PoolSize}",
            int.TryParse(Environment.GetEnvironmentVariable("EF_POOLING_TIMEOUT_SECONDS"), out var t) ? t : 30,
            poolSize);
    });

    // Debug logging for development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// ================== DEPENDENCY INJECTION ==================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();

logger.LogInformation("✓ Dependency Injection configured");

// ================== API CONFIGURATION ==================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "QLNhanVien API",
        Version = "v1.0.0",
        Description = "ASP.NET Core Web API - Employee Management System",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@example.com"
        }
    });
});

// CORS
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') ?? new[] { "http://localhost:3000", "http://localhost:5000" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

logger.LogInformation("✓ CORS configured for origins: {Origins}", string.Join(", ", allowedOrigins));

var app = builder.Build();

// ================== HTTP REQUEST PIPELINE ==================
logger.LogInformation("Building HTTP pipeline...");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QLNhanVien API V1");
        c.RoutePrefix = string.Empty;
    });
    logger.LogInformation("✓ Swagger UI enabled");
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthorization();
app.MapControllers();

logger.LogInformation("✓ HTTP pipeline configured");

// ================== DATABASE MIGRATION ==================
if (migrateOnStartup)
{
    try
    {
        logger.LogInformation("🔄 Checking database migrations...");
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                logger.LogWarning("⚠️  Found {Count} pending migrations: {Migrations}",
                    pendingMigrations.Count,
                    string.Join(", ", pendingMigrations));

                if (app.Environment.IsDevelopment())
                {
                    logger.LogInformation("🔧 Applying pending migrations...");
                    dbContext.Database.Migrate();
                    logger.LogInformation("✓ Migrations applied successfully");
                }
            }
            else
            {
                logger.LogInformation("✓ Database is up to date");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error during database migration: {Message}", ex.Message);
        logger.LogWarning("⚠️  App will continue without auto migration. Check DATABASE_URL/.env and rerun.");
    }
}
else
{
    logger.LogInformation("⏭️  Auto migration skipped (MIGRATE_ON_STARTUP=false)");
}

// ================== START APPLICATION ==================
logger.LogInformation("========================================");
logger.LogInformation("✨ Application started successfully!");
logger.LogInformation("📍 API available at: https://localhost:5001");
logger.LogInformation("📚 Swagger UI at: https://localhost:5001");
logger.LogInformation("========================================");

app.Run();

// ================== HELPERS ==================
static string GetHostFromConnectionString(string connectionString)
{
    var parts = connectionString.Split(';');
    var serverPart = parts.FirstOrDefault(p => p.StartsWith("Server=", StringComparison.OrdinalIgnoreCase));
    if (serverPart is not null)
    {
        return serverPart.Replace("Server=", "", StringComparison.OrdinalIgnoreCase);
    }

    var hostPart = parts.FirstOrDefault(p => p.StartsWith("Host=", StringComparison.OrdinalIgnoreCase));
    return hostPart?.Replace("Host=", "", StringComparison.OrdinalIgnoreCase) ?? "unknown";
}
