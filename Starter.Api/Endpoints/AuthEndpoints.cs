using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Starter.Api.Contracts.Auth;
using Starter.Api.RateLimiting;
using Starter.Application.Abstractions;
using Starter.Application.Features.Auth.Login;
using Starter.Application.Features.Auth.Register;

namespace Starter.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (
            RegisterRequest request,
            RegisterHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterCommand(request.Email, request.Password);
            var result = await handler.Handle(command, cancellationToken);

            if (result == RegisterResult.EmailAlreadyTaken)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Email is already taken.",
                    detail: "The provided email address is already registered."
                );
            }

            return Results.Created();
        });

        app.MapPost("/login", async (
            LoginRequest request,
            LoginHandler handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await handler.Handle(command, cancellationToken);

            if (!result.Success)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Invalid credentials.",
                    detail: "The provided email or password is incorrect."
                );
            }

            var user = result.User!;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Ok();
        }).RequireRateLimiting(RateLimitPolicies.Login);

        app.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.NoContent();
        });

        app.MapGet("/me", async (
            HttpContext httpContext,
            IUserRepository userRepository,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var user = await userRepository.GetUserById(userId, cancellationToken);

            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new { user.Id, Email = user.Email.Value });
        }).RequireAuthorization();
    }
}
