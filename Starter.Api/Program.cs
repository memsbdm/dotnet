using Microsoft.AspNetCore.Authentication.Cookies;
using Starter.Api.Data;
using Starter.Api.Endpoints;
using Starter.Api.Exceptions;
using Starter.Api.Health;
using Starter.Api.Observability;
using Starter.Api.RateLimiting;
using Starter.Application.Abstractions;
using Starter.Application.Features.Auth.Login;
using Starter.Application.Features.Auth.Register;
using Starter.Infrastructure;
using Starter.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiHealthChecks();
builder.Services.AddValidation();
builder.Services.AddApiRateLimiting(builder.Configuration);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapApiHealthChecks();

await app.InitialiseDevelopmentDataAsync();

app.Run();
