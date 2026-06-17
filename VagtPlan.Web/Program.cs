using VagtPlan.Web;
using VagtPlan.Web.Components;
using VagtPlan.Web.Services;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
