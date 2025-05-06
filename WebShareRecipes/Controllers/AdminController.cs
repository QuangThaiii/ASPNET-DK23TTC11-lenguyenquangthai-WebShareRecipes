using Microsoft.AspNetCore.Mvc;
using WebShareRecipes.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
namespace WebShareRecipes.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: /Admin/ManagePosts
        public IActionResult ManagePosts()
        {
            var recipes = _context.Recipes
                .Include(r => r.Category)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
            ViewBag.Users = _context.Users.ToList();
            return View(recipes);
        }

        // POST: /Admin/ApprovePost/5
        [HttpPost]
        public IActionResult ApprovePost(int id)
        {
            var recipe = _context.Recipes.Find(id);
            if (recipe != null)
            {
                recipe.IsApproved = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ManagePosts");
        }

        // POST: /Admin/DeletePost/5
        [HttpPost]
        public IActionResult DeletePost(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.Steps) // Include related RecipeSteps
                .FirstOrDefault(r => r.RecipeId == id);
            if (recipe != null)
            {
                // Delete related RecipeSteps first
                if (recipe.Steps != null && recipe.Steps.Any())
                {
                    _context.RecipeSteps.RemoveRange(recipe.Steps);
                }

                // Delete the recipe
                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
            }
            return RedirectToAction("ManagePosts");
        }

        // GET: /Admin/ManageUsers
        public IActionResult ManageUsers()
        {
            var users = _context.Users.OrderBy(u => u.Username).ToList();
            return View(users);
        }

        // GET: /Admin/EditUser/5
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                PasswordHash = null // không truyền mật khẩu ra ngoài
            };

            return View(viewModel);
        }

        // POST: /Admin/EditUser/5
        [HttpPost]
        public IActionResult EditUser(int id, EditUserViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.UserId != id && (u.Username == model.Username || u.Email == model.Email));
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên người dùng hoặc email đã tồn tại.");
                    return View(model);
                }

                var user = _context.Users.Find(id);
                if (user == null) return NotFound();

                user.Username = model.Username;
                user.Email = model.Email;
                user.IsAdmin = model.IsAdmin;

                // Chỉ đổi mật khẩu nếu người quản trị nhập mới
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    user.PasswordHash = HashPassword(model.PasswordHash); // Nên hash ở đây nếu có bảo mật
                }

                _context.SaveChanges();
                return RedirectToAction("ManageUsers");
            }

            return View(model);
        }

        // POST: /Admin/DeleteUser/5
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageUsers");
        }
        [HttpPost]
        public IActionResult ToggleAdmin(int id, bool isAdmin)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            user.IsAdmin = isAdmin;
            _context.SaveChanges();

            return Ok();
        }
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}