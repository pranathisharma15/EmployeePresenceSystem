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

var builder = WebApplication.CreateBuilder(args);   // This is the main entry point of the application. It sets up the web application builder, configures services, and defines the middleware pipeline for handling HTTP requests. The builder is used to register services such as controllers, database context, repositories, services, authentication, authorization, CORS policies, and Swagger for API documentation. After configuring the services and middleware, the application is built and run to start listening for incoming requests.

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT Support
builder.Services.AddSwaggerGen(options =>   // This configures Swagger to generate API documentation for the application. It defines a Swagger document with a title and version, and it also adds a security definition for JWT Bearer authentication. This allows developers to authenticate using JWT tokens directly from the Swagger UI when testing the API endpoints. The security requirement is added to ensure that the JWT token is included in the header of requests made through the Swagger UI for protected endpoints.
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
        Description = "Enter: Bearer <your-token>"
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

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>  
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IPresenceRepository, PresenceRepository>();  // This registers the PresenceRepository class as the implementation of the IPresenceRepository interface in the dependency injection container. It uses a scoped lifetime, which means that a new instance of PresenceRepository will be created for each HTTP request and shared within that request. This allows for efficient management of database connections and ensures that the repository is properly disposed of at the end of each request.
builder.Services.AddScoped<IPresenceService, PresenceService>();    
builder.Services.AddScoped<JwtService>();

// JWT Authentication
var key = Encoding.UTF8.GetBytes("sdfghjkloiuytrdsdfghjkoiuytrdsdfghjkoiuyt");

builder.Services.AddAuthentication(options =>   // This configures JWT Bearer authentication for the application. It sets the default authentication and challenge schemes to use JWT Bearer tokens. The AddJwtBearer method is used to specify the token validation parameters, including validating the issuer, audience, signing key, and token lifetime. The valid issuer and audience are set to "office-app", and the signing key is created using a symmetric security key derived from a secret string. The clock skew is set to zero to prevent any additional time allowance for token expiration.
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // This sets the default authentication scheme to JWT Bearer, which means that the application will use JWT tokens for authenticating incoming requests by default.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    // This sets the default challenge scheme to JWT Bearer, which means that if an unauthenticated request is made to a protected endpoint, the application will respond with a challenge indicating that JWT Bearer authentication is required.
})
.AddJwtBearer(options =>    // This configures the JWT Bearer authentication options, including the token validation parameters. It specifies that the issuer, audience, signing key, and token lifetime should be validated. The valid issuer and audience are set to "office-app", and the signing key is created using a symmetric security key derived from a secret string. The clock skew is set to zero to prevent any additional time allowance for token expiration.
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

// Authorization
builder.Services.AddAuthorization();    // This adds authorization services to the application, allowing for the use of authorization policies and attributes to control access to protected endpoints based on user roles or other criteria. With this in place, you can use the [Authorize] attribute on controllers or actions to restrict access to authenticated users or users with specific roles.

// CORS
builder.Services.AddCors(options => // This configures Cross-Origin Resource Sharing (CORS) policies for the application. It defines a policy named "AllowFrontend" that allows requests from specific origins (http://localhost:3000 and http://localhost:5173), which are commonly used for frontend development. The policy also allows any header and any method, enabling the frontend application to make API requests to the backend without being blocked by CORS restrictions.
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:5173"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");   

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();