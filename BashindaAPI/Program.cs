using System.Text;
using BashindaAPI.Data;
using BashindaAPI.Helpers;
using BashindaAPI.Models;
using BashindaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
    
    options.AddPolicy("AllowSpecific", builder =>
    {
        builder.WithOrigins(
                "https://localhost:7193", 
                "http://localhost:5072",
                "https://localhost:7196", // Additional possible URLs
                "http://localhost:5000",
                "https://localhost:5001",
                "http://localhost:5003"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bashinda API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bashinda API v1"));
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecific");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        // Execute SQL script to fix foreign key constraints
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "fix_foreign_keys.sql");
        if (File.Exists(scriptPath))
        {
            var script = File.ReadAllText(scriptPath);
            context.Database.ExecuteSqlRaw(script);
        }
        
        // Seed location data if needed
        if (!context.Divisions.Any())
        {
            // Sample divisions
            var dhaka = new Division { Name = "Dhaka" };
            var chittagong = new Division { Name = "Chittagong" };
            var khulna = new Division { Name = "Khulna" };
            var rajshahi = new Division { Name = "Rajshahi" };
            var sylhet = new Division { Name = "Sylhet" };

            context.Divisions.AddRange(dhaka, chittagong, khulna, rajshahi, sylhet);
            context.SaveChanges();

            // Sample districts for Dhaka
            var dhakaDistrict = new District { Name = "Dhaka", Division = dhaka };
            var gazipur = new District { Name = "Gazipur", Division = dhaka };
            var narayanganj = new District { Name = "Narayanganj", Division = dhaka };
            var narsingdi = new District { Name = "Narsingdi", Division = dhaka };
            var tangail = new District { Name = "Tangail", Division = dhaka };

            context.Districts.AddRange(dhakaDistrict, gazipur, narayanganj, narsingdi, tangail);
            context.SaveChanges();

            // Sample upazilas for Dhaka district
            var dhanmondi = new Upazila { Name = "Dhanmondi", District = dhakaDistrict };
            var mirpur = new Upazila { Name = "Mirpur", District = dhakaDistrict };
            var uttara = new Upazila { Name = "Uttara", District = dhakaDistrict };
            var gulshan = new Upazila { Name = "Gulshan", District = dhakaDistrict };
            var mohammadpur = new Upazila { Name = "Mohammadpur", District = dhakaDistrict };

            context.Upazilas.AddRange(dhanmondi, mirpur, uttara, gulshan, mohammadpur);
            context.SaveChanges();

            // Sample wards for different area types
            var dhanmondiWard1 = new Ward { Name = "Ward 1", Upazila = dhanmondi, AreaType = AreaType.CityCorporation };
            var dhanmondiWard2 = new Ward { Name = "Ward 2", Upazila = dhanmondi, AreaType = AreaType.CityCorporation };
            var dhanmondiUnion1 = new Ward { Name = "Union 1", Upazila = dhanmondi, AreaType = AreaType.Union };
            var dhanmondiUnion2 = new Ward { Name = "Union 2", Upazila = dhanmondi, AreaType = AreaType.Union };
            var dhanmondiPourasava = new Ward { Name = "Pourasava 1", Upazila = dhanmondi, AreaType = AreaType.Pourasava };

            context.Wards.AddRange(dhanmondiWard1, dhanmondiWard2, dhanmondiUnion1, dhanmondiUnion2, dhanmondiPourasava);
            context.SaveChanges();

            // Sample villages/areas
            var area1 = new Village { Name = "Shankar", Ward = dhanmondiWard1 };
            var area2 = new Village { Name = "Kalabagan", Ward = dhanmondiWard1 };
            var area3 = new Village { Name = "Jigatola", Ward = dhanmondiWard1 };
            var area4 = new Village { Name = "Sobhanbag", Ward = dhanmondiWard1 };
            var area5 = new Village { Name = "Science Lab", Ward = dhanmondiWard1 };

            context.Villages.AddRange(area1, area2, area3, area4, area5);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();
