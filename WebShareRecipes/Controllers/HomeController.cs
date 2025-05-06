using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShareRecipes.Models;
using Microsoft.AspNetCore.Mvc.Filters;
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

        public IActionResult Index()
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

            var viewModel = new IndexViewModel
            {
                Categories = categories,
                Recipes = null,
                IsSearchResult = false,
                SearchQuery = null,
                CurrentPage = 0,
                TotalPages = 0
            };

            ViewBag.Categories = _context.Categories.ToList();
            return View(viewModel);
        }

        public IActionResult ByCategory(int id, int? page)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            const int pageSize = 6;
            int pageNumber = page.HasValue ? page.Value : 1;

            var recipes = _context.Recipes
                .Where(r => r.CategoryId == id && r.IsApproved == true)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalRecipes = _context.Recipes
                .Count(r => r.CategoryId == id && r.IsApproved == true);
            int totalPages = (int)Math.Ceiling((double)totalRecipes / pageSize);

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

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
            ViewBag.Categories = _context.Categories.ToList();

            var viewModel = new IndexViewModel
            {
                Categories = null,
                IsSearchResult = true,
                SearchQuery = query,
                CurrentPage = 0,
                TotalPages = 0
            };

            if (string.IsNullOrEmpty(query))
            {
                const int pageSize = 10;
                int pageNumber = page.HasValue ? page.Value : 1;

                var allRecipes = _context.Recipes
                    .Where(r => r.IsApproved == true)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                int totalRecipes = _context.Recipes
                    .Count(r => r.IsApproved == true);
                int totalPages = (int)Math.Ceiling((double)totalRecipes / pageSize);

                viewModel.Recipes = allRecipes;
                viewModel.CurrentPage = pageNumber;
                viewModel.TotalPages = totalPages;
            }
            else
            {
                const int pageSizeSearch = 10;
                int pageNumberSearch = page.HasValue ? page.Value : 1;

                var recipes = _context.Recipes
                    .Where(r => r.IsApproved == true && r.Title.Contains(query))
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNumberSearch - 1) * pageSizeSearch)
                    .Take(pageSizeSearch)
                    .ToList();

                int totalRecipesSearch = _context.Recipes
                    .Count(r => r.IsApproved == true && r.Title.Contains(query));
                int totalPagesSearch = (int)Math.Ceiling((double)totalRecipesSearch / pageSizeSearch);

                viewModel.Recipes = recipes;
                viewModel.CurrentPage = pageNumberSearch;
                viewModel.TotalPages = totalPagesSearch;
            }

            return View("Index", viewModel);
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