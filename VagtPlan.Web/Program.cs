using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using VagtPlan.Web;
using VagtPlan.Web.Components;
using VagtPlan.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var danishCulture = CultureInfo.GetCultureInfo("da-DK");
CultureInfo.DefaultThreadCurrentCulture = danishCulture;
CultureInfo.DefaultThreadCurrentUICulture = danishCulture;

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<VagtPlan.Web.Services.UserRoleApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice/");
    });

builder.Services.AddHttpClient<VagtPlan.Web.Services.UserApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

// Ensure UserApiClient can be resolved directly from DI as well
builder.Services.AddScoped<VagtPlan.Web.Services.UserApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var authState = sp.GetRequiredService<VagtPlan.Web.Services.ApiAuthState>();
    var client = factory.CreateClient();
    client.BaseAddress = new("https+http://apiservice");
    return new VagtPlan.Web.Services.UserApiClient(client, authState);
});

builder.Services.AddHttpClient<VagtPlan.Web.Services.DepartmentApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

builder.Services.AddHttpClient<VagtPlan.Web.Services.WorkTimeApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

// StatusService
builder.Services.AddHttpClient<StatusService>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https+http://apiservice";
    client.BaseAddress = new(baseUrl);
});

// WishService
builder.Services.AddHttpClient<WishService>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https+http://apiservice";
    client.BaseAddress = new(baseUrl);
});

// Simple user client for selecting users
builder.Services.AddHttpClient<UserService>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https+http://apiservice";
    client.BaseAddress = new(baseUrl);
});

builder.Services.AddHttpClient<WorkDayService>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https+http://apiservice";
    client.BaseAddress = new(baseUrl);
});

builder.Services.AddHttpClient<VagtPlan.Web.Services.AuthApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

builder.Services.AddScoped<VagtPlan.Web.Services.ApiAuthState>();

// Persist DataProtection keys to a directory inside the app (survives restarts/containers)
var dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");
Directory.CreateDirectory(dataProtectionKeysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("VagtPlan");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// When running behind a reverse proxy (Portainer / Traefik / nginx), enable forwarded headers
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// Accept forwarded headers from any proxy in containerized environment
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

// Redirect to HTTPS only when a port is configured or when TLS is terminated by a reverse proxy.
// In Docker/Portainer we set ASPNETCORE_HTTPS_PORT=443 in compose to avoid the warning.
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
