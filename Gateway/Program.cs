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

            var cert = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "client.pfx"), "1234");

            builder.Services.AddReverseProxy()
                .ConfigureHttpClient((context, handler) =>
                {
                    handler.SslOptions.ClientCertificates = new X509CertificateCollection
                    (
                        new[] { cert }
                    );
                })
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
            
            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapReverseProxy();
            app.Run();
        }
    }
}