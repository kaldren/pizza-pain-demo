using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Components.Layout;

namespace pizza.web.tests.Authentication;

// Covers the app-shell sign-in/sign-out widget (acceptance criteria: an unauthenticated user sees
// a Sign in action; an authenticated user sees their name + Sign out, and Sign in is hidden).
public class LoginDisplayTests : BunitContext
{
    public LoginDisplayTests()
    {
        // The Authorized branch renders an <AntiforgeryToken/>, which resolves an
        // AntiforgeryStateProvider from DI. Provide a no-op stub so the component renders in tests.
        Services.AddSingleton<AntiforgeryStateProvider>(new StubAntiforgeryStateProvider());
    }

    [Fact]
    public void WhenNotAuthenticated_ShowsSignInLink_AndHidesSignOut()
    {
        // Arrange
        AddAuthorization().SetNotAuthorized();

        // Act
        var cut = Render<LoginDisplay>();

        // Assert
        var signIn = cut.Find("a.signin-link");
        Assert.Equal("Sign in", signIn.TextContent.Trim());
        Assert.Equal("login", signIn.GetAttribute("href"));
        Assert.Empty(cut.FindAll("button.logout-btn"));
    }

    [Fact]
    public void WhenAuthenticated_ShowsUserName_AndSignOut_AndHidesSignIn()
    {
        // Arrange
        AddAuthorization().SetAuthorized("Jane Doe");

        // Act
        var cut = Render<LoginDisplay>();

        // Assert
        Assert.Equal("Jane Doe", cut.Find(".user-name").TextContent.Trim());

        var logoutForm = cut.Find("form.logout-form");
        Assert.Equal("authentication/logout", logoutForm.GetAttribute("action"));
        Assert.Equal("post", logoutForm.GetAttribute("method"));
        Assert.Equal("Sign out", cut.Find("button.logout-btn").TextContent.Trim());

        Assert.Empty(cut.FindAll("a.signin-link"));
    }

    private sealed class StubAntiforgeryStateProvider : AntiforgeryStateProvider
    {
        public override AntiforgeryRequestToken? GetAntiforgeryToken() => null;
    }
}
