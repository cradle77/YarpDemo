using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

namespace Demo_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var cert = new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "client.pfx"), "1234");

            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.ConfigureHttpsDefaults(options =>
                {
                    options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                });
            });

            builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                    options.Events = new CertificateAuthenticationEvents()
                    {
                        OnCertificateValidated = ctx =>
                        {
                            if (ctx.ClientCertificate.Thumbprint == "4C48545EC86DE0FECC2C7ABF3F50A3FBCCD4DACF")
                            {
                                Console.WriteLine("Certs match!");
                                ctx.Success();
                            }
                            else
                            {
                                Console.WriteLine("certs don't match");
                                ctx.Fail("wrong cert");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/healthz");

            app.MapControllers();

            app.Run();
        }
    }
}