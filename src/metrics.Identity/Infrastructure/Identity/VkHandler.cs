using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using metrics.Identity.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace metrics.Identity.Infrastructure.Identity
{
    public class VkHandler : OAuthHandler<VkOauthOptions>
    {
        public VkHandler(IOptionsMonitor<VkOauthOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity,
            AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var endpoint =
                QueryHelpers.AddQueryString(Options.UserInformationEndpoint, "access_token", tokens.AccessToken);
            endpoint = QueryHelpers.AddQueryString(endpoint, "v", Options.ApiVersion);

            if (Options.Fields.Count > 0)
            {
                endpoint = QueryHelpers.AddQueryString(endpoint, "fields", string.Join(',', Options.Fields));
            }

            var response = await Backchannel.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"An error occurred when retrieving Facebook user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding Facebook Graph API is enabled.");
            }

            using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, user.RootElement.GetProperty("response")[0]);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
    }
}