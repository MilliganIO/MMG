using Microsoft.FluentUI.AspNetCore.Components;
using MmgExplorer.Components;
using MmgExplorer.Services;
using MmgExplorer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Added for FluentUI Components to work
builder.Services.AddFluentUIComponents();


// HTTP client for API communication
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl configuration is missing.");

builder.Services.AddHttpClient<IApiClient, ApiClient>("MmgatApi",
    client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });
builder.Services.AddScoped<IGuideClient, GuideClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
