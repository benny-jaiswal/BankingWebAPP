using BankingWebApp.Providers;
using BankingWebApp.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IAuthService, AuthService>(); // Register AuthService
builder.Services.AddScoped<IBankingService, BankingService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // Register Custom Auth State
builder.Services.AddScoped<ProtectedSessionStorage>();

//  Register the JWT Authorization Handler
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient("BankingAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>(); // Attach JWT automatically


builder.Services.AddAuthorizationCore(); // Add Authorization Core

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
