using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShareRecipes.Models;
using System.Linq;

namespace WebShareRecipes.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
            : base(context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string query, int? page)
        {
            ViewBag.Categories = _context.Categories.ToList();
            var viewModel = new IndexViewModel
            {
                Categories = null,
                Recipes = null,
                IsSearchResult = !string.IsNullOrEmpty(query),
                SearchQuery = query,
                CurrentPage = 0,
                TotalPages = 0
            };

            if (!string.IsNullOrEmpty(query))
            {
                const int pageSize = 10;
                int pageNumber = page.HasValue ? page.Value : 1;
                query = query.Trim().ToLower();

                var recipes = _context.Recipes
                    .Include(r => r.Category)
                    .Where(r => r.IsApproved == true &&
                        (r.Title.ToLower().Contains(query) ||
                         r.Ingredients.ToLower().Contains(query) ||
                         r.Category.Name.ToLower().Contains(query)))
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                int totalRecipes = _context.Recipes
                    .Count(r => r.IsApproved == true &&
                        (r.Title.ToLower().Contains(query) ||
                         r.Ingredients.ToLower().Contains(query) ||
                         r.Category.Name.ToLower().Contains(query)));
                int totalPages = (int)Math.Ceiling((double)totalRecipes / pageSize);

                viewModel.Recipes = recipes;
                viewModel.CurrentPage = pageNumber;
                viewModel.TotalPages = totalPages;
            }
            else
            {
                var categories = _context.Categories
                    .Select(c => new CategoryWithRecipesViewModel
                    {
                        Category = c,
                        Recipes = _context.Recipes
                            .Where(r => r.CategoryId == c.CategoryId && r.IsApproved == true)
                            .OrderByDescending(r => r.CreatedAt)
                            .Take(5)
                            .ToList()
                    })
                    .ToList();

                viewModel.Categories = categories;
            }

            return View(viewModel);
        }

        public IActionResult ByCategory(int id, string query, int? page)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            const int pageSize = 6;
            int pageNumber = page.HasValue ? page.Value : 1;

            var recipesQuery = _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.CategoryId == id && r.IsApproved == true);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.Trim().ToLower();
                recipesQuery = recipesQuery.Where(r =>
                    r.Title.ToLower().Contains(query) ||
                    r.Ingredients.ToLower().Contains(query));
            }

            var recipes = recipesQuery
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalRecipes = recipesQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecipes / pageSize);

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = query;

            return View(recipes);
        }

        public IActionResult Details(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.Steps)
                .Include(r => r.Category)
                .FirstOrDefault(r => r.RecipeId == id && r.IsApproved == true);
            if (recipe == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == recipe.UserId);
            ViewBag.Username = user != null ? user.Username : "Không xác định";
            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        public IActionResult Search(string query, int? page)
        {
            return Index(query, page); // Chuyển hướng về Index với query
        }

        public IActionResult Privacy()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}