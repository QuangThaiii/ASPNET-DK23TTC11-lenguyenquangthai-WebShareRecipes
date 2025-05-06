using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShareRecipes.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chủ đề.")]
        public string Name { get; set; }

        [Display(Name = "Hiển thị")]
        public bool Status { get; set; } = true;
    }
}