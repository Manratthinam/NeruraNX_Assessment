using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using neuranx.Api.Filters;
using neuranx.Application;
using neuranx.Domain;
using neuranx.Domain.RequestModel;
using neuranx.Infrastructure;
using System.Text;
using System.Text.Json;


// Configure Log4Net LogDirectory
var logDir = Path.Combine(Directory.GetCurrentDirectory(), "log");
var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
while (dir != null)
{
    if (dir.GetFiles("*.sln").Any())
    {
        logDir = Path.Combine(dir.FullName, "log");
        break;
    }
    dir = dir.Parent;
}

if (!Directory.Exists(logDir))
{
    Directory.CreateDirectory(logDir);
}

log4net.GlobalContext.Properties["LogDirectory"] = logDir.Replace("\\", "/");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Logging.AddLog4Net("log4net.config");

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthorization();
builder.Services.AddCors(builder =>
{
    builder.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();

    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.FromMinutes(3),
    };
});

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers(option =>
{
    option.Filters.Add<ResponseFilter>();
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<neuranx.Infrastructure.Persistence.ApplicationDbContext>();
        if (db.Database.GetPendingMigrations().Any())
            db.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/Auth/register"))
    {
        await ConvertModelAsync<RequestDto, RegisterModel>(context);
    }
    else if (context.Request.Path.StartsWithSegments("/api/Auth/login"))
    {
        await ConvertModelAsync<RequestDto, LoginModel>(context);
    }

    await next();
});

app.UseExceptionHandler(opt => { });

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();

async Task ConvertModelAsync<TWrapper, TInner>(HttpContext context)
{
    context.Request.EnableBuffering();

    using var reader = new StreamReader(
        context.Request.Body,
        Encoding.UTF8,
        leaveOpen: true);

    var requestBody = await reader.ReadToEndAsync();
    context.Request.Body.Position = 0;

    var wrapper = JsonSerializer.Deserialize<TWrapper>(requestBody);
    Console.WriteLine($"[Middleware] Wrapper: {System.Text.Json.JsonSerializer.Serialize(wrapper)}");

    var payload = typeof(TWrapper)
        .GetProperty("payload")?
        .GetValue(wrapper);

    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var innerModel = JsonSerializer.Deserialize<TInner>(
        JsonSerializer.Serialize(payload), options
    );

    var modifiedJson = JsonSerializer.Serialize(innerModel);
    var bytes = Encoding.UTF8.GetBytes(modifiedJson);

    context.Request.Body = new MemoryStream(bytes);
    context.Request.ContentLength = bytes.Length;
    context.Request.Body.Position = 0;
}
