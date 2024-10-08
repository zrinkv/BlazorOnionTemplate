using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using BlazorServer.UI.Components;
using BlazorServer.UI.SharedServices;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLocalization(); //localization

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("MKVodovodAPI", httpClient =>
{
    httpClient.BaseAddress = new Uri(Configuration["WebAPIUrl"]);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IBaseHttpClient, BaseHttpClient>();
builder.Services.AddScoped<Radzen.NotificationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.Cookie.Name = "auth_cookie";
        options.LoginPath = "/login";
        options.Cookie.MaxAge = TimeSpan.FromHours(24);
        options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration["SessionToken:ExpiresInMinutes"]));
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        //options.Cookie.SameSite = SameSiteMode.Lax;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

//Localization
string[] supportedCultures = Configuration.GetSection("SupportedCultures").Get<string[]>();
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
app.UseStatusCodePagesWithRedirects("/StatusCode/404");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
