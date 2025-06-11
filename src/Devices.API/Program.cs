using System.Text;
using Devices.API;
using Devices.API.Helpers.Middleware;
using Devices.Services;
using Devices.Entities.DTOs;
using Devices.Services.Options;
using Devices.Services.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtConfigData = builder.Configuration.GetSection("Jwt");
var connectionString = builder.Configuration.GetConnectionString("PersonalDatabase") ?? 
                       throw new InvalidOperationException("Personal connection string not found");
builder.Services.AddDbContext<DeviceDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.Configure<JwtOptions>(jwtConfigData);

builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAccountService, AccountService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfigData["Issuer"],
            ValidAudience = jwtConfigData["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigData["Key"])),
            ClockSkew = TimeSpan.FromMinutes(10)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<DeviceValidationMiddleware>();

app.MapControllers();

app.Run();

