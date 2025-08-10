using AuthService.Application.Common.Interfaces;
using AuthService.Infrastructure.ExternalServices;
using AuthService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using AuthService.Application.Common.Behaviors;
using FluentValidation;
using System.Reflection;
using AuthService.Api.Filters;
using AuthService.Infrastructure.JWT;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using System.Text;
using AuthService.Application.Services.Users.Command;
using AuthService.Application.Services.Users.Query;
using AuthService.Application.Services.Users.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// DbContext
builder.Services.AddDbContext<AuthDbContext>(opt =>
{
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("AuthDb"),
        sql =>
        {
            sql.MigrationsHistoryTable("__EFMigrationsHistory", "auth");
            sql.EnableRetryOnFailure();
        });
});

// Bridge DbContext برای Application
builder.Services.AddScoped<IAuthDbContext>(sp => sp.GetRequiredService<AuthDbContext>());

// MediatR: اسمبلی Application را ثبت کن
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AuthService.Application.Services.Users.Command.RegisterUserCommand).Assembly);
});

// ثبت همه Validatorها از اسمبلی Application
builder.Services.AddValidatorsFromAssembly(
    typeof(AuthService.Application.Services.Users.Command.RegisterUserCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddControllers(o =>
{
    o.Filters.Add<ResultStatusFilter>(); // global
});

// پیاده‌سازی‌های زیرساخت
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

//EmailSender
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// JWT Auth (برای محافظت از سایر endpointها)
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
