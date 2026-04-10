using GNT.Domain.Models;
using GNT.Enums;
using GNT.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GNT.Infrastructure;

public static class DatabaseSeeder
{
    public static void SeedDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();

        if (db.Database.IsRelational())
            db.Database.Migrate();

        SeedRoles(roleManager, db);
        SeedAdminUser(userManager, roleManager, config, logger);
        SeedProducts(db);

        db.SaveChanges();
    }

    private static void SeedRoles(RoleManager<Role> roleManager, AppDbContext db)
    {
        if (db.Roles.Any()) return;

        var allPermissions = Enum.GetValues<Permission>()
            .Select(p => new RolePermission { PermissionId = p })
            .ToArray();

        var adminRole = new Role
        {
            Name = "Administrator",
            IsDefaultRole = true,
            RolePermissions = allPermissions
        };

        var userRole = new Role
        {
            Name = "User",
            IsDefaultRole = true,
            RolePermissions =
            [
                new RolePermission { PermissionId = Permission.ViewUsers },
                new RolePermission { PermissionId = Permission.ViewRoles },
                new RolePermission { PermissionId = Permission.ViewProducts }
            ]
        };

        roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
        roleManager.CreateAsync(userRole).GetAwaiter().GetResult();
    }

    private static void SeedAdminUser(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IConfiguration config,
        ILogger logger)
    {
        if (userManager.Users.Any()) return;

        var adminEmail = config["Seed:AdminEmail"]
            ?? throw new InvalidOperationException("Seed:AdminEmail nu este configurat.");

        var adminPassword = config["Seed:AdminPassword"]
            ?? throw new InvalidOperationException("Seed:AdminPassword nu este configurat.");

        var admin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Template",
            EmailConfirmed = true
        };

        userManager.CreateAsync(admin, adminPassword).GetAwaiter().GetResult();
        userManager.AddToRoleAsync(admin, "Administrator").GetAwaiter().GetResult();

        logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }

    private static void SeedProducts(AppDbContext db)
    {
        if (db.BusinessProduct.Any()) return;

        var products = new List<BusinessProduct>();
        var random = new Random();
        var types = Enum.GetValues<BusinessProductType>();

        for (int i = 1; i <= 500; i++)
        {
            var dateIn = DateTime.Today.AddDays(-random.Next(1, 365));
            products.Add(new BusinessProduct
            {
                Name = $"Product {i}",
                Price = Math.Round((decimal)(random.NextDouble() * 1000), 2),
                Type = types[random.Next(types.Length)],
                IsInStock = random.Next(2) == 1,
                DatetIn = dateIn,
                DateOut = dateIn.AddDays(random.Next(1, 100))
            });
        }

        db.BusinessProduct.AddRange(products);
    }
}