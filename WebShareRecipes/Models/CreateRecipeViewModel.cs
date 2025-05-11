using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShareRecipes.Models
{
    public class CreateRecipeViewModel
    {
        public RecipeViewModel Recipe { get; set; }
        public List<RecipeStepViewModel> RecipeSteps { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public string Version => "2025-05-06-NoRequired";
    }

    public class RecipeViewModel
    {
        public int? RecipeId { get; set; }

        [Required(ErrorMessage = "Tên món ăn là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên món ăn không được dài quá 100 ký tự.")]
        public string Title { get; set; }

        public string Description { get; set; } // Không bắt buộc

        public string Ingredients { get; set; } // Không bắt buộc

        public string Instructions { get; set; } // Không bắt buộc

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int? CategoryId { get; set; }

        public string ImageUrl { get; set; }
    }

    public class RecipeStepViewModel
    {
        [Required(ErrorMessage = "Tiêu đề bước là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tiêu đề bước không được dài quá 200 ký tự.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả bước là bắt buộc.")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int StepOrder { get; set; }
    }
}