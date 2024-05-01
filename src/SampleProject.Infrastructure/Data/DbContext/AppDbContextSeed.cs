using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleProject.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure.Data.DbContext
{
    public class AppDbContextSeed
    {
        public static async Task SeedAsync(AppDbContext appDbContext,
            ILoggerFactory loggerFactory, IServiceProvider service, int? retry = 0)
        {
            try
            {
                var roleManager = service.GetRequiredService<RoleManager<ApplicationRole>>
                    ();
                var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                await PopulateRoles(roleManager);
                await PopulateUser(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<AppDbContext>();
                logger.LogError(ex, "An error occurred seeding AppDbContext DB tables.");
            }

        }
        private static async Task PopulateRoles(RoleManager<ApplicationRole> RoleManager)
        {
            string[] roleNames = { "SuperAdmin" }; //please don't modify the existing data

            foreach (var roleName in roleNames)
            {

                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }
        }

        private static async Task PopulateUser(UserManager<ApplicationUser> UserManager)
        {
            foreach (var userItem in GetPreconfiguredUser())
            {
                var user = await UserManager.FindByEmailAsync(userItem.Email);

                if (user == null)
                {
                    string userPassword = "P@ssw0rd@12345";
                    var createUserResult = await UserManager.CreateAsync(userItem, userPassword);
                    if (createUserResult.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(userItem, "SuperAdmin");
                    }
                }
            }
        }


        static IEnumerable<ApplicationUser> GetPreconfiguredUser()
        {
            return new List<ApplicationUser>() {
                new ApplicationUser() { UserName = "user@sifio.com",Email = "user@sifio.com", EmailConfirmed = true, PhoneNumberConfirmed=true},

            };
        }
    }
}
