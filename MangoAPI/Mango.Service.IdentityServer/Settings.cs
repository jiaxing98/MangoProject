using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Mango.Service.IdentityServer
{
    public static class Settings
    {
        public const string ADMIN = "Admin";
        public const string CUSTOMER = "Customer";

        public static IEnumerable<IdentityResource> identityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> apiScopes =>
            new List<ApiScope>
            {
                new ApiScope("mango", "Mango Server"),
                new ApiScope("read", "Read"),
                new ApiScope("write", "Write"),
                new ApiScope("delete", "Delete"),
            };

        public static IEnumerable<Client> clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "read", "write", "profile" }
                },
                new Client
                {
                    ClientId = "mango",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:7267/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7267/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "mango"
                    }
                },
            };
    }
}
