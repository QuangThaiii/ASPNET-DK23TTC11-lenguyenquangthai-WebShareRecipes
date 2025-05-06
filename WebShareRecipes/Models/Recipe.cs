using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShareRecipes.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public string? Ingredients { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsApproved { get; set; } = false;
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<RecipeStep>? Steps { get; set; }
    }
}