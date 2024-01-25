using Microsoft.AspNetCore.Identity;
using System;

namespace Sklep_z_towarami.Data
{
    public class MyIdentityDataInitializer
    {

        public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] rolesArray = { "Admin", "Customer" };

            SeedRoles(roleManager, rolesArray);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            SeedUser(userManager, "admin@localhost", "admin", "Admin");
            SeedUser(userManager, "customer@localhost", "customer", "Customer");
        }

        public static void SeedUser(UserManager<IdentityUser> userManager, string name, string password, string role = null)
        {
            if (userManager.FindByEmailAsync(name).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = name,
                    Email = name
                };
                IdentityResult result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded && role != null)
                {
                    userManager.AddToRoleAsync(user, role).Wait();
                }
                
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, string[] rolesArray)
        {
            foreach (string roleName in rolesArray) {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole
                    {
                        Name = "Admin",
                    };
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }
        }
    }
}
