using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CalcAPI.Infrastructure.Authentication;

/// <summary>
/// The ApiKeyAuthenticationHandler is responsible for authenticating requests using an API key. Using C# primary constructor syntax
/// </summary>
public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    IConfiguration configuration
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    private const string ApiKeyHeaderName = "ApiKey";
    private readonly string validApiKey = configuration["ApiKey"];

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key missing"));
        }

        if (apiKey != validApiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var identity = new ClaimsIdentity("ApiKey");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "ApiKey");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}