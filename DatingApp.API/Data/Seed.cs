using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
  public class Seed
  {
    public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
      if (!userManager.Users.Any())
      {
        var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
        var users = JsonConvert.DeserializeObject<List<User>>(userData);

        // create some roles
        var roles = new List<Role>
        {
          new Role{Name = "Member"},
          new Role{Name = "Admin"},
          new Role{Name = "Moderator"},
          new Role{Name = "VIP"}
        };

        foreach (var role in roles)
        {
          roleManager.CreateAsync(role).Wait();
        }
        foreach (var user in users)
        {
          // all seeded users will have the same password
          user.Photos.SingleOrDefault().IsApproved = true;
          userManager.CreateAsync(user, "password").Wait();
          userManager.AddToRoleAsync(user, "Member").Wait();
        }

        // create admin user
        var adminUser = new User
        {
          UserName = "Kamila",
          KnownAs = "Admin"
        };
     
        IdentityResult result = userManager.CreateAsync(adminUser, "password00").Result;
        if (result.Succeeded)
        {
          var admin = userManager.FindByNameAsync("Kamila").Result;
          //userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
          userManager.AddToRoleAsync(admin, "Admin").Wait();
          userManager.AddToRoleAsync(admin, "Moderator").Wait();
        }        
      }
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }
  }
}