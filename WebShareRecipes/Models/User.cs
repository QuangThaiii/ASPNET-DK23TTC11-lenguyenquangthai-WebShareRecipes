using System;
using System.Collections.Generic;

namespace WebShareRecipes.Models
{
    public class User
    {
        //public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Recipe> Recipes { get; set; }
        public ICollection<RecipeComment> Comments { get; set; }
    }

}