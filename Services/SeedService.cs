using Microsoft.AspNetCore.Identity;
using RoleBasedAuthorization.Data;
using RoleBasedAuthorization.Models;

namespace RoleBasedAuthorization.Services
{
    public class SeedService
    {
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var logger = services.GetRequiredService<ILogger<SeedService>>();

            try
            {
                logger.LogInformation("Seeding database...");

                // Ensure the database is created
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database created or already exists.");

                // Seed roles
                logger.LogInformation("Seeding roles...");
                await SeedRolesAsync(roleManager, "Admin");
                await SeedRolesAsync(roleManager, "User");

                // add admin user
                logger.LogInformation("Seeding admin user...");
                var adminEmail = "admin@engineer.com";
                var adminFound = await userManager.FindByEmailAsync(adminEmail);
                if (adminFound == null)
                {
                    var adminUser = new Users
                    {
                        FullName = "Hopeless Engineer",
                        UserName = adminEmail,
                        Email = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        NormalizedUserName = adminEmail.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Admin user created and assigned to Admin role.");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }  

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    throw new Exception($"Role '{roleName}' created successfully.");
                }
                else
                {
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"Role '{roleName}' already exists.");
            }
        } 
    }
}