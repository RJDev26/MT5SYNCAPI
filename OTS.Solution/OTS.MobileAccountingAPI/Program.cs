using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OTS.DOMAIN.Database;
using OTS.Service;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<AccountingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register your custom services
builder.Services.AddApplicationDependencies(builder.Configuration);

// Register CORS (Allow Any Origin/Method/Header)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS (must be before UseAuthorization/MapControllers)
app.UseCors(); // applies the default policy

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
