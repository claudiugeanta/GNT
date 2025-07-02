using GNT.Dtos.Enums;
using GNT.Infrastructure.Context;
using GNT.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GNT.Shared.Enums;

namespace GNT.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(this WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                var appDbContext = services.GetRequiredService<AppDbContext>();

                if (appDbContext.Database.IsRelational())
                {
                    appDbContext.Database.Migrate();
                }

                if (appDbContext.User.Count() == 0)
                {
                    var adminRole = new Role
                    {
                        Name = "Administrator",
                        IsDefaultRole = true,
                        RolePermissions = new[]
                        {
                            new RolePermission() {PermissionId = Permission.ViewUsers},
                            new RolePermission() {PermissionId = Permission.ManageUsers},
                            new RolePermission() {PermissionId = Permission.ViewRoles},
                            new RolePermission() {PermissionId = Permission.ManageRoles},
                            new RolePermission() {PermissionId = Permission.ViewProducts},
                            new RolePermission() {PermissionId = Permission.ManageProducts}
                        }

                    };

                    var userRole = new Role
                    {
                        Name = "User",
                        IsDefaultRole = true,
                        RolePermissions = new[]
                        {
                            new RolePermission() {PermissionId = Permission.ViewUsers},
                            new RolePermission() {PermissionId = Permission.ManageUsers},
                            new RolePermission() {PermissionId = Permission.ViewRoles},
                            new RolePermission() {PermissionId = Permission.ManageRoles},
                            new RolePermission() {PermissionId = Permission.ViewProducts},
                            new RolePermission() {PermissionId = Permission.ManageProducts}
                        }
                    };

                    var admin = new User
                    {
                        Email = "claudiugeanta@gmail.com",
                        FirstName = "Cvu",
                        LastName = "Template",
                        CreatedAt = new DateTime(2018, 1, 1),
                        LastUpdatedAt = new DateTime(2018, 1, 1),
                        //Parola11a#
                        Password = "Ad+PeWsjpYteZQB4As2eTX+Rsd9WT1aYslx0jAJtcT1K0a5M6LSUl2NPeWVYihAxoA==",
                        UserRoles = new[] { new UserRole() { Role = adminRole } }
                    };


                    appDbContext.Role.AddRange(adminRole, userRole);
                    appDbContext.User.AddRange(admin);

                }

                if (appDbContext.Product.Count() == 0)
                {
                    var products = SeedProducts(500);

                    appDbContext.AddRange(products);
                }

                appDbContext.SaveChanges();
            }
        }

        private static List<Product> SeedProducts(int count = 100)
        {
            var products = new List<Product>();
            var random = new Random();
            var types = Enum.GetValues(typeof(ProductType));

            for (int i = 1; i <= count; i++)
            {
                var dateIn = DateTime.Today.AddDays(-random.Next(1, 365));
                var dateOut = dateIn.AddDays(random.Next(1, 100));

                var product = new Product
                {
                    Name = $"Product {i}",
                    Code = $"{RandomLetters(2, random)}{random.Next(10000, 100000)}",
                    Price = Math.Round((decimal)(random.NextDouble() * 1000), 2),
                    Type = (ProductType)types.GetValue(random.Next(types.Length)),
                    IsInStock = random.Next(2) == 1,
                    DatetIn = dateIn,
                    DateOut = dateOut
                };

                products.Add(product);
            }

            return products;
        }

        private static string RandomLetters(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
