using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BlogApplication.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApplication.Data
{
    public static class DefaultDataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedDefaultUsers(userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                await roleManager.CreateAsync(adminRole);
            }

            if (!await roleManager.RoleExistsAsync("Editor"))
            {
                var editorRole = new IdentityRole("Editor");
                await roleManager.CreateAsync(editorRole);
            }
        }

        private static async Task SeedDefaultUsers(UserManager<ApplicationUser> userManager)
        {
            // Create an Admin user if it doesn't exist
            var adminUser = await userManager.FindByEmailAsync("admin@blogapp.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@blogapp.com",
                    Email = "admin@blogapp.com",
                    FullName = "Admin User",
                    Role = "Admin"
                };
                var createAdminResult = await userManager.CreateAsync(adminUser, "Admin@1234");
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create an Editor user if it doesn't exist
            var editorUser = await userManager.FindByEmailAsync("editor@blogapp.com");
            if (editorUser == null)
            {
                editorUser = new ApplicationUser
                {
                    UserName = "editor@blogapp.com",
                    Email = "editor@blogapp.com",
                    FullName = "Editor User",
                    Role = "Editor"
                };
                var createEditorResult = await userManager.CreateAsync(editorUser, "Editor@1234");
                if (createEditorResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(editorUser, "Editor");
                }
            }
        }
    }
}
