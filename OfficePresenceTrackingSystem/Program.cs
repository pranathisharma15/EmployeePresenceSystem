using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using OfficePresenceTrackingSystem.Data;
using OfficePresenceTrackingSystem.Repositories;
using OfficePresenceTrackingSystem.Repositories.Interfaces;
using OfficePresenceTrackingSystem.Services;
using OfficePresenceTrackingSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ✅ Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger + JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Office Presence API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter token as: Bearer <token>"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ✅ SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ✅ Dependency Injection
builder.Services.AddScoped<IPresenceRepository, PresenceRepository>();
builder.Services.AddScoped<IPresenceService, PresenceService>();
builder.Services.AddScoped<JwtService>();

// ✅ JWT Authentication
var key = Encoding.UTF8.GetBytes(
    "sdfghjkloiuytrdsdfghjkoiuytrdsdfghjkoiuyt"
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidIssuer = "office-app",
        ValidAudience = "office-app",

        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// ✅ Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// ✅ Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ IMPORTANT ORDER
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();