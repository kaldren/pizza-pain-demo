using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace pizza.web.Infrastructure.Authentication;

/// <summary>
/// Maps the minimal-API endpoints that start the Entra ID sign-in challenge and the sign-out flow.
/// The UI links to <c>authentication/login</c> and posts to <c>authentication/logout</c>.
/// </summary>
internal static class LogInOrOutEndpointRouteBuilderExtensions
{
    internal static IEndpointConventionBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        // Anonymous: redirect the user agent to Entra ID to authenticate. The OIDC scheme is named
        // explicitly because the default challenge scheme is the cookie (which points at "/login").
        group.MapGet("/login", (string? returnUrl) =>
                TypedResults.Challenge(
                    GetAuthProperties(returnUrl),
                    [OpenIdConnectDefaults.AuthenticationScheme]))
            .AllowAnonymous();

        // Sign out of both the cookie session and the OIDC handler. Signing out of the OIDC handler
        // ensures the user is not silently signed back in on the next authenticated request.
        group.MapPost("/logout", ([FromForm] string? returnUrl) =>
            TypedResults.SignOut(
                GetAuthProperties(returnUrl),
                [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

        return group;
    }

    private static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        // Prevent open redirects: only ever return to a local path within the app.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = pathBase;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{pathBase}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}
