using Microsoft.Extensions.DependencyInjection;
using WebShareRecipes.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class DbInitializer
{
    public static void SeedAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Users.Any(u => u.Username == "myAdmin"))
        {
            var admin = new User
            {
                Username = "myAdmin",
                Email = "admin@example.com",
                PasswordHash = HashPassword("my@dmin"),
                IsAdmin = true,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(admin);
            context.SaveChanges();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
