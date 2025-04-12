using System.Collections.Generic;
using System.Data.Entity;
using Models;
public class MyDbContext : DbContext
{
    public MyDbContext() : base("DefaultConnection")
    {
    }

    public DbSet<User> Users { get; set; } // Định nghĩa bảng Products
}