namespace metrics.Options
{
    public class VkontakteOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public const string TokenEndpoint = "https://oauth.vk.com/access_token";
        public const string AuthorizationEndpoint = "https://oauth.vk.com/authorize";
        public const string UserInformationEndpoint = "https://api.vk.com/method/users.get.json";
    }
}
