using security.Models;
using Microsoft.AspNetCore.Identity;

namespace security.Data;

public class Seed
{
    public static async Task SeedData(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager )
    {

        string[] roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin";
        var adminUser = await userManager.FindByNameAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.se",
                Email = "admin@admin.se"
            };

            var result = await userManager.CreateAsync(adminUser, "Pa$$w0rd");
            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return;
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        var users = new List<(User user, string[] Roles)>
{
    (new User { FirstName="Lucas", LastName="Andersson", UserName="lucas.andersson@gmail.com", Email="lucas.andersson@gmail.com" }, ["User"]),
    (new User { FirstName="Amira", LastName="Hassan", UserName="amira.hassan@gmail.com", Email="amira.hassan@gmail.com" }, ["User"]),
    (new User { FirstName="Jonathan", LastName="Berg", UserName="jonathan.berg@gmail.com", Email="jonathan.berg@gmail.com" }, ["User"])
};

        foreach (var (user, userRoles) in users)
        {
            var existingUser = await userManager.FindByNameAsync(user.UserName!);

            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, "Pa$$w0rd");

                if (!result.Succeeded)
                {
                    Console.WriteLine($"Kunde inte skapa {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    continue;
                }

                foreach (var role in userRoles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }


        if (context.Todos.Any()) return;

        var todos = new List<Todo>
        {
          new()
    {
        Title = "Assemble IKEA Billy Bookshelf",
         },
    new()
    {
        Title = "Install Philips Hue Smart Bulbs",
          },
    new()
    {
        Title = "Organize Kitchen with Storage Bins",
          }
        };

        context.Todos.AddRange(todos);
        await context.SaveChangesAsync();
    }
}

