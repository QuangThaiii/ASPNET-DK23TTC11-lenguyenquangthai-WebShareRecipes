using System.ComponentModel.DataAnnotations;

namespace WebShareRecipes.Models
{
    public class RecipeStep
    {
        public int RecipeStepId { get; set; }
        [Required(ErrorMessage = "Mô tả bước là bắt buộc")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Tiêu đề bước là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề bước không được dài quá 200 ký tự")]
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public int StepOrder { get; set; }
        [Required]
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
    }
}