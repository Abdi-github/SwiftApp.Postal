using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using StackExchange.Redis;
using SwiftApp.Postal.Modules.Address;
using SwiftApp.Postal.Modules.Auth;
using SwiftApp.Postal.Modules.Branch;
using SwiftApp.Postal.Modules.Delivery;
using SwiftApp.Postal.Modules.Notification;
using SwiftApp.Postal.Modules.Parcel;
using SwiftApp.Postal.Modules.Tracking;
using SwiftApp.Postal.SharedKernel.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;
using SwiftApp.Postal.SharedKernel.Services;
using SwiftApp.Postal.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ─────────────────────────────────────────────
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// ── DbContext ────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SwiftApp.Postal.WebApi")));

// Register EF Core configurations from all module assemblies
AppDbContext.ConfigurationAssemblies.AddRange([
    typeof(AuthModuleInstaller).Assembly,
    typeof(BranchModuleInstaller).Assembly,
    typeof(AddressModuleInstaller).Assembly,
    typeof(ParcelModuleInstaller).Assembly,
    typeof(DeliveryModuleInstaller).Assembly,
    typeof(TrackingModuleInstaller).Assembly,
    typeof(NotificationModuleInstaller).Assembly,
]);

// ── HTTP Context + Current User ──────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ── Module Installers ────────────────────────────────────
new AuthModuleInstaller().Install(builder.Services, builder.Configuration);
new BranchModuleInstaller().Install(builder.Services, builder.Configuration);
new AddressModuleInstaller().Install(builder.Services, builder.Configuration);
new ParcelModuleInstaller().Install(builder.Services, builder.Configuration);
new DeliveryModuleInstaller().Install(builder.Services, builder.Configuration);
new TrackingModuleInstaller().Install(builder.Services, builder.Configuration);
new NotificationModuleInstaller().Install(builder.Services, builder.Configuration);

// ── MediatR ──────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<AuthModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<BranchModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<AddressModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<ParcelModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<DeliveryModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<TrackingModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<NotificationModuleInstaller>();
    cfg.RegisterServicesFromAssemblyContaining<Program>(); // WebApi event handlers
});

// ── Redis Distributed Cache ──────────────────────────────
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6380";
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "postal:";
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// ── Quartz Scheduler ─────────────────────────────────────
builder.Services.AddQuartz(q =>
{
    // Retry failed notifications every 5 minutes
    var retryJobKey = new JobKey("RetryFailedNotificationsJob", "Notification");
    q.AddJob<SwiftApp.Postal.Modules.Notification.Infrastructure.Jobs.RetryFailedNotificationsJob>(retryJobKey);
    q.AddTrigger(t => t
        .ForJob(retryJobKey)
        .WithIdentity("RetryFailedNotificationsTrigger", "Notification")
        .WithCronSchedule("0 0/5 * * * ?"));

    // Daily digest at 06:00 UTC (= 07:00 Zurich winter)
    var digestJobKey = new JobKey("DailyNotificationDigestJob", "Notification");
    q.AddJob<SwiftApp.Postal.Modules.Notification.Infrastructure.Jobs.DailyNotificationDigestJob>(digestJobKey);
    q.AddTrigger(t => t
        .ForJob(digestJobKey)
        .WithIdentity("DailyNotificationDigestTrigger", "Notification")
        .WithCronSchedule("0 0 6 * * ?"));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ── SignalR ──────────────────────────────────────────────
builder.Services.AddSignalR();
builder.Services.AddScoped<SwiftApp.Postal.Modules.Notification.Domain.Interfaces.INotificationHubPusher,
    SwiftApp.Postal.WebApi.Infrastructure.NotificationHubPusher>();

// ── Controllers ──────────────────────────────────────────
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuthModuleInstaller).Assembly)
    .AddApplicationPart(typeof(BranchModuleInstaller).Assembly)
    .AddApplicationPart(typeof(AddressModuleInstaller).Assembly)
    .AddApplicationPart(typeof(ParcelModuleInstaller).Assembly)
    .AddApplicationPart(typeof(DeliveryModuleInstaller).Assembly)
    .AddApplicationPart(typeof(TrackingModuleInstaller).Assembly)
    .AddApplicationPart(typeof(NotificationModuleInstaller).Assembly);

// ── Swagger / OpenAPI ────────────────────────────────────
var keycloakAuthority = builder.Configuration["Keycloak:Authority"]!;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SwiftApp Postal API", Version = "v1", Description = "Swiss Postal System REST API" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile" },
                    { "email", "Email address" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            ["openid", "profile", "email"]
        }
    });
});

// ── CORS ─────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                ?? ["http://localhost:5101"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── ProblemDetails (RFC 7807) ─────────────────────────────
builder.Services.AddProblemDetails();

// ── Health Checks ─────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis");

// ── Authentication (Keycloak JWT Bearer) ─────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Extract Keycloak realm_access.roles into ClaimTypes.Role claims
                if (context.Principal?.Identity is not ClaimsIdentity identity) return Task.CompletedTask;
                // var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtBearer");
                // logger.LogDebug("Token validated for Subject={Subject}, Name={Name}", context.Principal.FindFirst("sub")?.Value, context.Principal.Identity?.Name);

                var realmAccessClaim = context.Principal.FindFirst("realm_access")?.Value;
                if (realmAccessClaim is null) return Task.CompletedTask;

                try
                {
                    using var doc = JsonDocument.Parse(realmAccessClaim);
                    if (doc.RootElement.TryGetProperty("roles", out var roles))
                    {
                        // logger.LogDebug("realm_access contains {RoleCount} role entries", roles.GetArrayLength());
                        foreach (var role in roles.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (roleValue is not null)
                                identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                            // System.Diagnostics.Debug.WriteLine($"Mapped role claim: {roleValue}");
                        }
                    }
                }
                catch { /* Ignore malformed realm_access claim */ }

                return Task.CompletedTask;
            }
        };
    });

// ── Authorization Policies ────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("ADMIN", "BRANCH_MANAGER"));
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

// ─────────────────────────────────────────────────────────
var app = builder.Build();
// ─────────────────────────────────────────────────────────

// Redirect 0.0.0.0 to localhost — browsers don't handle cookies for 0.0.0.0 properly.
app.Use(async (context, next) =>
{
    if (context.Request.Host.Host == "0.0.0.0")
    {
        var port = context.Request.Host.Port ?? 5100;
        var newUrl = $"{context.Request.Scheme}://localhost:{port}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(newUrl, permanent: true);
        return;
    }
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwiftApp Postal API v1");
        c.OAuthClientId(builder.Configuration["Keycloak:ClientId"]);
        c.OAuthUsePkce();
    });
}

app.UseSerilogRequestLogging();
app.UseMiddleware<SwiftApp.Postal.SharedKernel.Middleware.GlobalExceptionMiddleware>();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<SwiftApp.Postal.WebApi.Hubs.NotificationHub>("/hubs/notifications");

app.Run();
