using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Version: Updated to remove [Required] on Categories, ImageUrl, and RecipeSteps.ImageUrl
// Artifact ID: 7e90d57f-f220-4603-8ec5-a8a6906c119b
// Debug Version: 2025-05-06
namespace WebShareRecipes.Models
{
    public class CreateRecipeViewModel
    {
        public RecipeViewModel Recipe { get; set; }
        public List<RecipeStepViewModel> RecipeSteps { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public string Version => "2025-05-06-NoRequired"; // Debug version
    }

    public class RecipeViewModel
    {
        public int? RecipeId { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được dài quá 100 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Nguyên liệu là bắt buộc")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Hướng dẫn là bắt buộc")]
        public string Instructions { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int? CategoryId { get; set; }

        public string ImageUrl { get; set; }
    }

    public class RecipeStepViewModel
    {
        [Required(ErrorMessage = "Tiêu đề bước là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề bước không được dài quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả bước là bắt buộc")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int StepOrder { get; set; }
    }
}