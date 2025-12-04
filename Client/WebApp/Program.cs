using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApp.Authentication;
using WebApp.Components;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5177") });
builder.Services.AddScoped<HttpCompanyRepresentativeService>();
builder.Services.AddScoped<HttpApplicantService>();
builder.Services.AddScoped<HttpCompanyService>();
builder.Services.AddScoped<HttpRecruiterService>();
builder.Services.AddScoped<AuthProvider>();
builder.Services.AddScoped<HttpApplicationService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthProvider>());
builder.Services.AddScoped<HttpJobListingService>();
builder.Services.AddAuthentication("Cookies").AddCookie();
builder.Services.AddAuthorization();


    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication(); app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();