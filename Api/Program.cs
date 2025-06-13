using BusinessLogic.Mapping;
using BusinessLogic.Services;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using DataAccess;
using DataAccess.Repositories;
using EncryptedNotes.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

//Service
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();