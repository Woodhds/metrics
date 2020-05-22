namespace Base.Contracts.Options
{
    public class IdentityOptions
    {
        public string ServerUrl { get; set; }
        public string UserToken => ServerUrl + "/api/admin/usertoken";
    }
}