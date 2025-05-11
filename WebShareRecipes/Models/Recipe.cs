using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShareRecipes.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Tên món ăn là bắt buộc.")]
        public string Title { get; set; }

        public string? Description { get; set; } // Không bắt buộc
        public string? Instructions { get; set; } // Không bắt buộc
        public string? Ingredients { get; set; } // Không bắt buộc
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsApproved { get; set; } = false;
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
        public ICollection<RecipeStep>? Steps { get; set; }
    }
}