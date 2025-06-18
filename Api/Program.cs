using System.Text;
using BusinessLogic.Mapping;
using BusinessLogic.Services;
using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using DataAccess;
using DataAccess.Repositories;
using EncryptedNotes.Configurations;
using EncryptedNotes.Mapping;
using EncryptedNotes.Middlewares;
using EncryptedNotes.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

//Cache
builder.Services.AddMemoryCache();

//EF DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Automapper
builder.Services.AddAutoMapper(typeof(BusinessLogicMappingProfile), typeof(ApiMappingProfile));

//Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Service BusinessLogic
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddSingleton<ICryptoService, CryptoService>();
builder.Services.AddSingleton<ISignatureService, SignatureService>();

//Service Infrastructure
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

//Configurazione JWT
var jwtSettingsConfigurationSection = builder.Configuration.GetSection("JwtSettings");
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(jwtSettingsConfigurationSection)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var jwtSettings = jwtSettingsConfigurationSection.Get<JwtSettings>();
ArgumentNullException.ThrowIfNull(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.PrivateKey)),
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
        };
    });

//Build
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();