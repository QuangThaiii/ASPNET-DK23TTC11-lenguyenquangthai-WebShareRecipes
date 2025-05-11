using Microsoft.AspNetCore.Mvc;
using WebShareRecipes.Models;

namespace WebShareRecipes.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _context.Categories.Where(c => c.Status == true).ToList();
            return View(_context.Categories.ToList());
        }

        //public IActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //public IActionResult Edit(int id)
        //{
        //    var category = _context.Categories.Find(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        [HttpPost]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.Status = !category.Status;
            _context.SaveChanges();

            return Ok(new { status = category.Status });
        }
    }
}