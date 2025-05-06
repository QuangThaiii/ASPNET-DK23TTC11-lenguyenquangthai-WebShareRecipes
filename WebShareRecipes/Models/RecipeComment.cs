using System;
using System.Collections.Generic;

namespace WebShareRecipes.Models
{
    public class RecipeComment
    {
        //public int Id { get; set; }
        public int RecipeCommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}