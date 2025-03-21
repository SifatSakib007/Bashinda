using Bashinda.Data;
using Bashinda.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure API HttpClient
builder.Services.AddHttpClient("BashindaAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5003/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect = true,
    MaxAutomaticRedirections = 10,
    UseDefaultCredentials = false,
    UseCookies = true
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register API services
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IRenterProfileApiService, RenterProfileApiService>();
builder.Services.AddScoped<IApartmentOwnerProfileApiService, ApartmentOwnerProfileApiService>();
builder.Services.AddScoped<IApartmentApiService, ApartmentApiService>();
builder.Services.AddScoped<IAdminApiService, AdminApiService>();
builder.Services.AddScoped<ApiHealthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Register custom services
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IRenterProfileService, RenterProfileService>();
builder.Services.AddScoped<ILocationDataService, LocationDataService>();

builder.Services.AddScoped<IAdminLocationService, AdminLocationService>();

// Simplified DbContext for local caching only
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalCacheConnection")));

// Authentication configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    context.Response.Redirect(context.RedirectUri);
                }
                return Task.CompletedTask;
            }
        };
    });

// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "Bashinda.Session";
});

var app = builder.Build();

// Configure the HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Ensure uploads directory exists
try
{
    var uploadsDir = Path.Combine(app.Environment.WebRootPath, "uploads");
    Directory.CreateDirectory(uploadsDir);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Error creating uploads directory");
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication/Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();