using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using System.Security.Cryptography.X509Certificates;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddAuthorization(configure =>
            {
                configure.AddPolicy("authenticated", policy => policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme));

                configure.AddPolicy("authenticatedApi", policy => policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Cookies"));
            });
            
            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapReverseProxy();
            app.Run();
        }
    }
}