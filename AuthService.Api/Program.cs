using AuthService.Api.Filters;
using AuthService.Application.Common.Behaviors;
using AuthService.Application.Common.Interfaces;
using AuthService.Application.Services.Users.Command;
using AuthService.Application.Services.Users.DTOs;
using AuthService.Application.Services.Users.Query;
using AuthService.Infrastructure.Context;
using AuthService.Infrastructure.ExternalServices;
using AuthService.Infrastructure.JWT;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Security;
using AuthService.SharedKernel.Messaging.Contracts.Auth;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

#region RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // در صورت نیاز Consumerها را اینجا AddConsumer کن
    // x.AddConsumer<SomeConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["Rabbit:Host"], "/", h =>
        {
            h.Username(builder.Configuration["Rabbit:User"]);
            h.Password(builder.Configuration["Rabbit:Pass"]);
        });

        // اگر Consumer داری:
        // cfg.ReceiveEndpoint("auth-service-queue", e =>
        // {
        //     e.ConfigureConsumer<SomeConsumer>(context);
        // });
    });
});
builder.Services.AddScoped<IMessageBus, MassTransitMessageBus>();
#endregion

#region Controllers&Swagger
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region DbContext
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
#endregion

#region MediatR
// MediatR: اسمبلی Application را ثبت کن
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AuthService.Application.Services.Users.Command.RegisterUserCommand).Assembly);
});
#endregion

#region JWT
//EmailSender
builder.Services.AddSingleton<IEmailSender, EmailSender>();
// پیاده‌سازی‌های زیرساخت
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
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
#endregion


var app = builder.Build();

#region ValidatorPipeLine
// ثبت همه Validatorها از اسمبلی Application
builder.Services.AddValidatorsFromAssembly(
    typeof(AuthService.Application.Services.Users.Command.RegisterUserCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddControllers(o =>
{
    o.Filters.Add<ResultStatusFilter>(); // global
});
#endregion

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
