using EasyDatabaseManager.Configuration;
using EasyDatabaseManager.Endpoints;
using ExpenseFlow.Api.Extension;
using ExpenseFlow.Api.Middleware;
using ExpenseFlow.Application.Services.AutoAssignment;
using ExpenseFlow.Application.Services.Email;
using ExpenseFlow.Application.Services.Excel;
using ExpenseFlow.Application.Services.File;
using ExpenseFlow.Application.Services.Helper;
using ExpenseFlow.Application.Services.Token;
using ExpenseFlow.Infrastructure.Data;
using ExpenseFlow.Infrastructure.Seeder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// ---- Kestrel hardening ----
builder.WebHost.ConfigureKestrel(o =>
{
    o.AddServerHeader = false;
    o.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    o.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(75);
    o.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

// ---- Services ----
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<IAutoAssignmentService, AutoAssignmentService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSingleton(sp => new ParsingConfig
{
    CustomTypeProvider = new DynamicLinqCustomTypeProvider(),
});

builder.Services.AddHttpClient();

var jwtSettings = new JwtSettings
{
    Key = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("Jwt:Key is missing."),
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "ExpenseFlow",
    Audience = builder.Configuration["Jwt:Audience"] ?? "ExpenseFlowClient",
    ExpirationMinutes = builder.Configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60
};
builder.Services.AddSingleton(jwtSettings);

QuestPDF.Settings.License = LicenseType.Community;

// ---- Localization ----
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("ar"),
        new CultureInfo("en")
    };

    options.DefaultRequestCulture = new RequestCulture("ar", "ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// ---- DbContext ----
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseMySQL(connectionString);

//    if (builder.Environment.IsDevelopment())
//    {
//        options.EnableSensitiveDataLogging();
//        options.EnableDetailedErrors();
//    }
//});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});




// ---- MediatR ----
builder.Services.AddMediatR(s =>
{
    s.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

// ---- Easy Database Manager ----
builder.Services.AddEasyDatabaseManager(options =>
{
    options.ConnectionString = connectionString;
    options.DatabaseType = DatabaseType.PostgreSql;
    options.RoutePrefix = "/sql";
    options.EnableWriteOperations = builder.Environment.IsDevelopment();
    options.SoftDeleteColumn = "IsValid";
    options.AccessKey = builder.Configuration["DatabaseManager:AccessKey"]
        ?? "CHANGE_THIS_DATABASE_MANAGER_ACCESS_KEY";
    options.AccessSessionDuration = new TimeSpan(1, 0, 0, 0);
});

// ---- Authentication / Authorization ----
var isDev = builder.Environment.IsDevelopment();
builder.Services
    .AddServices()
    .AddSwaggerService()
    .AddJwtService(builder.Configuration, requireHttpsMetadata: !isDev);

// ---- CORS ----
const string FrontendCors = "FrontendCors";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, p =>
    {
        if (allowedOrigins.Length > 0)
        {
            p.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            p.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

// ---- Rate Limiting ----
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var ip = ctx.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";

        return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 300,
            TokensPerPeriod = 100,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            AutoReplenishment = true,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.AddPolicy("Tight", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            }));
});

var app = builder.Build();

// ---- Localization ----
var locOpts = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOpts.Value);

// ---- Seeder ----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = services.GetRequiredService<IWebHostEnvironment>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<AppDbContext>();

        var migrations = db.Database.GetMigrations().ToList();
        if (migrations.Count == 0)
        {
            logger.LogWarning("No EF Core migrations exist yet. Create InitialCreate before seeding the database.");
        }
        else
        {
            if (!env.IsProduction())
            {
                var pending = await db.Database.GetPendingMigrationsAsync();
                if (pending.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied.");
                }
                else
                {
                    logger.LogInformation("No pending migrations.");
                }
            }

            await Seeder.SeedData(services);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

// ---- Exception handling ----
if (!isDev)
    app.UseHsts();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuditLogMiddleware>();

app.UseHttpsRedirection();

// ---- Security Headers ----
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
    ctx.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    await next();
});

// ---- Static Files ----
app.UseStaticFiles();

// ---- Swagger ----
if (isDev)
{
    app.UseSwagger();

    app.UseSwaggerUI(s =>
    {
        s.RoutePrefix = "swagger";
        s.SwaggerEndpoint("v1/swagger.json", "All");
        s.SwaggerEndpoint("Trainee/swagger.json", "Trainee");
        s.SwaggerEndpoint("dashboard/swagger.json", "Dashboard");
        s.SwaggerEndpoint("other/swagger.json", "Other");
        s.DocExpansion(DocExpansion.List);
        s.DisplayRequestDuration();
        s.EnableTryItOutByDefault();
        s.DocumentTitle = "ExpenseFlow App API Documentation";
    });

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("ExpenseFlow App API")
            .WithTheme(ScalarTheme.Kepler)
            .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json")
            .AddPreferredSecuritySchemes(new List<string> { "Bearer" })
            .AddHttpAuthentication("Bearer", auth =>
            {
                // auth.Token = "";
            })
            .EnablePersistentAuthentication();
    });
}

// ---- JWT for Swagger ----
app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/swagger"), branch =>
{
    branch.UseAuthentication();
    branch.UseAuthorization();
});

// ---- Rate Limiting ----
app.UseRateLimiter();

// ---- CORS ----
app.UseCors(FrontendCors);

// ---- Auth ----
app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

// ---- Health checks ----
app.MapHealthChecks("/health").AllowAnonymous();

// ---- Controllers ----
app.MapControllers().RequireRateLimiting("Tight");

// ---- Easy Database Manager ----
app.MapEasyDatabaseManager();

app.Run();