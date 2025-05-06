using System.ComponentModel.DataAnnotations;

namespace WebShareRecipes.Models
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        [DataType(DataType.Password)]
        public string? PasswordHash { get; set; }
    }

}