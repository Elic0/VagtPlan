using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using VagtPlan.Web;
using VagtPlan.Web.Components;
using VagtPlan.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https+http://apiservice";
if (!apiBaseUrl.EndsWith('/'))
{
    apiBaseUrl += "/";
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var danishCulture = CultureInfo.GetCultureInfo("da-DK");
CultureInfo.DefaultThreadCurrentCulture = danishCulture;
CultureInfo.DefaultThreadCurrentUICulture = danishCulture;

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

void ConfigureApiClient(HttpClient client) => client.BaseAddress = new Uri(apiBaseUrl);

builder.Services.AddHttpClient<VagtPlan.Web.Services.UserRoleApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<VagtPlan.Web.Services.UserApiClient>(ConfigureApiClient);

// Ensure UserApiClient can be resolved directly from DI as well
builder.Services.AddScoped<VagtPlan.Web.Services.UserApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var authState = sp.GetRequiredService<VagtPlan.Web.Services.ApiAuthState>();
    var client = factory.CreateClient();
    client.BaseAddress = new Uri(apiBaseUrl);
    return new VagtPlan.Web.Services.UserApiClient(client, authState);
});

builder.Services.AddHttpClient<VagtPlan.Web.Services.DepartmentApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<VagtPlan.Web.Services.WorkTimeApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<StatusService>(ConfigureApiClient);
builder.Services.AddHttpClient<WishService>(ConfigureApiClient);
builder.Services.AddHttpClient<UserService>(ConfigureApiClient);
builder.Services.AddHttpClient<WorkDayService>(ConfigureApiClient);
builder.Services.AddHttpClient<VagtPlan.Web.Services.AuthApiClient>(ConfigureApiClient);

builder.Services.AddScoped<VagtPlan.Web.Services.ApiAuthState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
