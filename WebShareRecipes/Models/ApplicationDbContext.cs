using System;
using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;
    namespace WebShareRecipes.Models
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Recipe> Recipes { get; set; } = null!;
            public DbSet<Category> Categories { get; set; } = null!;
            public DbSet<RecipeComment> RecipeComments { get; set; } = null!;
            public DbSet<RecipeStep> RecipeSteps { get; set; } = null!;
            public DbSet<User> Users { get; set; } = null!;
        }
    }

