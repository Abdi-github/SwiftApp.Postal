using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
using SwiftApp.Postal.WebApp.Components.Layout;
using SwiftApp.Postal.WebApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Kestrel — increase header limit for Keycloak JWT cookies ──
builder.WebHost.ConfigureKestrel(opts =>
{
    opts.Limits.MaxRequestHeadersTotalSize = 1024 * 128; // 128 KB (default 32 KB)
    opts.Limits.MaxRequestHeaderCount = 200;
});

// ── Serilog ─────────────────────────────────────────────
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// ── DbContext ────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ── SignalR ──────────────────────────────────────────────
builder.Services.AddSignalR();

// ── Blazor SSR ───────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Localization ─────────────────────────────────────────
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// ── Health Checks ─────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis");

// ── Authentication (Keycloak OIDC + Cookie) ───────────────
var keycloakAuthority = builder.Configuration["Keycloak:Authority"]!;
// In Docker dev, the webapp container uses the internal Docker hostname (keycloak:8080) to
// communicate with Keycloak server-to-server, but the BROWSER needs to be redirected to the
// externally accessible URL (e.g. 127.0.0.1:8090). Set Keycloak:PublicAuthority in compose
// to override the browser-facing redirect URL without breaking server-side token exchange.
var keycloakPublicAuthority = builder.Configuration["Keycloak:PublicAuthority"] ?? keycloakAuthority;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = "postal.session";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
    options.Cookie.MaxAge = TimeSpan.FromHours(8);
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = keycloakAuthority;
    options.ClientId = builder.Configuration["Keycloak:ClientId"];
    options.ClientSecret = builder.Configuration["Keycloak:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SaveTokens = true;
    // Keycloak includes all needed claims (sub, name, email, preferred_username, realm_access)
    // in the ID token when profile/email scopes are requested. The back-channel UserInfo call
    // would fail because the token's iss (browser-facing 127.0.0.1:8090) doesn't match the
    // UserInfo endpoint host (internal keycloak:8080), causing a 401. Disable it.
    options.GetClaimsFromUserInfoEndpoint = false;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.TokenValidationParameters.NameClaimType = "preferred_username";
    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    // Accept tokens issued by either the internal Docker URL or the browser-facing URL
    options.TokenValidationParameters.ValidIssuers =
    [
        keycloakAuthority,
        keycloakPublicAuthority
    ];
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            // Extract Keycloak realm_access.roles into ClaimTypes.Role claims.
            // Keycloak places realm_access in the access token, not the ID token, so we
            // decode the access token to get the roles when not found in the ID token.
            if (context.Principal?.Identity is not ClaimsIdentity identity) return Task.CompletedTask;

            var realmAccessJson = context.Principal.FindFirst("realm_access")?.Value;

            if (realmAccessJson is null)
            {
                var accessTokenStr = context.TokenEndpointResponse?.AccessToken;
                if (accessTokenStr is not null)
                {
                    try
                    {
                        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        if (handler.CanReadToken(accessTokenStr))
                        {
                            var accessToken = handler.ReadJwtToken(accessTokenStr);
                            realmAccessJson = accessToken.Claims
                                .FirstOrDefault(c => c.Type == "realm_access")?.Value;
                        }
                    }
                    catch { /* Ignore unreadable access token */ }
                }
            }

            if (realmAccessJson is null) return Task.CompletedTask;

            try
            {
                using var doc = JsonDocument.Parse(realmAccessJson);
                if (doc.RootElement.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (roleValue is not null)
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                    }
                }
            }
            catch { /* Ignore malformed realm_access claim */ }

            return Task.CompletedTask;
        },
        OnRedirectToIdentityProvider = context =>
        {
            // Replace Docker-internal authority with the externally accessible URL
            // so browser redirects go to the correct host (not the container-only hostname).
            if (keycloakPublicAuthority != keycloakAuthority)
            {
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress
                    .Replace(keycloakAuthority, keycloakPublicAuthority, StringComparison.OrdinalIgnoreCase);
            }
            return Task.CompletedTask;
        },
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            if (keycloakPublicAuthority != keycloakAuthority
                && context.ProtocolMessage.IssuerAddress is not null)
            {
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress
                    .Replace(keycloakAuthority, keycloakPublicAuthority, StringComparison.OrdinalIgnoreCase);
            }
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/login?error=remote");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("ADMIN", "BRANCH_MANAGER"));
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddCascadingAuthenticationState();

// ── WebApp Services ──────────────────────────────────────
builder.Services.AddScoped<SwiftApp.Postal.WebApp.Services.DashboardService>();
builder.Services.AddScoped<SwiftApp.Postal.Modules.Notification.Domain.Interfaces.INotificationHubPusher,
    SwiftApp.Postal.WebApp.Infrastructure.NullNotificationHubPusher>();

// ─────────────────────────────────────────────────────────
var app = builder.Build();
// ─────────────────────────────────────────────────────────

// Redirect 0.0.0.0 to localhost — browsers don't handle cookies for 0.0.0.0 properly,
// which breaks OIDC correlation cookies and causes authentication failures.
app.Use(async (context, next) =>
{
    if (context.Request.Host.Host == "0.0.0.0")
    {
        var port = context.Request.Host.Port ?? 5101;
        var newUrl = $"{context.Request.Scheme}://localhost:{port}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(newUrl, permanent: true);
        return;
    }
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();
app.UseStaticFiles();

var supportedCultures = new[] { "de-CH", "fr-CH", "it-CH", "en" };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("de-CH"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    RequestCultureProviders =
    [
        new CookieRequestCultureProvider { CookieName = "postal-locale" },
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    ]
});

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Redirect root to the app dashboard
app.MapGet("/", () => Results.Redirect("/app"));

// Auth endpoints for OIDC login/logout
app.MapGet("/login", (string? returnUrl) =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = returnUrl ?? "/app" },
        [OpenIdConnectDefaults.AuthenticationScheme]));

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties { RedirectUri = "/" });
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
