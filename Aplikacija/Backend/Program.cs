using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebTemplate.Hubs;
using WebTemplate.Models;
using WebTemplate.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Controllers + JSON (enum kao string)
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // SignalR - DODAJTE OVO PRE JWT
    builder.Services.AddSignalR();

    // JWT
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"]
        ?? throw new ArgumentNullException("JwtSettings:SecretKey is not configured");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // VAŽNO: OVO JE POTREBNO ZA SIGNALR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // Proverite da li je zahtev za SignalR hub
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/chatHub"))
                {
                    // Uzmi token iz query string-a
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization();

    // Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IChatService, ChatService>(); // OVO VEĆ IMATE
    builder.Services.AddLogging();

    // CORS - AŽURIRAJTE OVO
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173") // DODAJTE HTTPS
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .SetIsOriginAllowed((host) => true); // DODAJTE OVO
        });
    });

    // Swagger + JWT u Swagger-u
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebTemplate API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowAll");
    //app.UseHttpsRedirection();

    // VAŽNO: StaticFiles mora biti PRE Authentication
    app.UseStaticFiles();

    // VAŽNO: Authentication i Authorization moraju biti pre SignalR
    app.UseAuthentication();
    app.UseAuthorization();

    // Map SignalR hub - OVO JE ISPRAVNO
    app.MapHub<ChatHub>("/chatHub");

    app.MapControllers();

    // Seed
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var authService = services.GetRequiredService<IAuthService>();

            context.Database.Migrate();

            if (!await context.Korisnici.AnyAsync())
            {
                var adminUser = new Korisnik
                {
                    Ime = "Admin",
                    Prezime = "Admin",
                    Username = "admin",
                    Email = "admin@webtemplate.com",
                    PasswordHash = authService.HashPassword("Admin123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    Biografija = "",
                    SlikaURL = "",
                    Telefon = "",
                    VestineJson = ""
                };

                context.Korisnici.Add(adminUser);
                await context.SaveChangesAsync();
                Console.WriteLine("Admin user created: admin / Admin123!");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Došlo je do greške prilikom inicijalizacije baze.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Startup error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}