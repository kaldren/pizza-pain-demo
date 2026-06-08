using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Features.Authentication.Pages;

namespace pizza.web.tests.Features.Authentication;

// Covers the sign-in page (acceptance criteria: a clean Sign in form whose action starts the Entra
// ID challenge, returning the user to where they came from after authenticating).
public class LoginPageTests : BunitContext
{
    [Fact]
    public void RendersMicrosoftSignInButton_LinkingToChallengeEndpoint()
    {
        // Act
        var cut = Render<Login>();

        // Assert
        var button = cut.Find("a.ms-signin");
        Assert.Contains("Sign in with Microsoft", button.TextContent);
        Assert.Equal("authentication/login", button.GetAttribute("href"));
    }

    [Fact]
    public void WithReturnUrl_ForwardsReturnUrlToChallengeEndpoint()
    {
        // Arrange — SupplyParameterFromQuery is fed from the navigation URI, not a direct parameter.
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo(nav.GetUriWithQueryParameter("ReturnUrl", "/counter"));

        // Act
        var cut = Render<Login>();

        // Assert
        var button = cut.Find("a.ms-signin");
        Assert.Equal("authentication/login?returnUrl=%2Fcounter", button.GetAttribute("href"));
    }
}
