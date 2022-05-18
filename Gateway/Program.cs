using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

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

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
            
            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapReverseProxy();
            app.Run();
        }
    }
}