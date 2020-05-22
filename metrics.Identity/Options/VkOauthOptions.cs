using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace metrics.Identity.Options
{
    public class VkOauthOptions : OAuthOptions
    {
        public VkOauthOptions()
        {
            AuthorizationEndpoint = "https://oauth.vk.com/authorize";
            UserInformationEndpoint = "https://api.vk.com/method/users.get.json";
            TokenEndpoint = "https://oauth.vk.com/access_token";
        }

        public string ApiVersion { get; set; }
        public HashSet<string> Fields { get; set; } = new HashSet<string>();
    }
}