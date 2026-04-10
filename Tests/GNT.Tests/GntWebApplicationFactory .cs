using GNT.Application.Account.Utils;
using GNT.Application.Interfaces;
using GNT.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GNT.Tests.Infrastructure;

public class GntWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TokenProvider:Issuer"] = "testserver",
                ["TokenProvider:Audience"] = "testclient",
                ["TokenProvider:ExpirationSeconds"] = "9000",
                ["TokenProvider:CookieName"] = "jwt-cookie",
                ["ClientAppUrl"] = "https://localhost:7086",
                ["AllowedOrigins:0"] = "https://localhost:7086"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Gaseste si elimina TOATE serviciile adaugate de AddInfrastructureServices
            // inclusiv cele interne EF Core pentru SQL Server
            var descriptors = services
                .Where(d => d.ServiceType.Assembly.FullName != null &&
                            d.ServiceType.Assembly.FullName.Contains("EntityFrameworkCore"))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            var dbDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                            d.ServiceType == typeof(DbContextOptions) ||
                            d.ServiceType == typeof(AppDbContext) ||
                            d.ServiceType == typeof(IAppDbContext))
                .ToList();

            foreach (var d in dbDescriptors)
                services.Remove(d);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("GntTestDb"));

            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());
        });
    }
}