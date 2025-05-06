using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShareRecipes.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebShareRecipes.Controllers
{
    public class RecipesController : BaseController
    {
        public RecipesController(ApplicationDbContext context) : base(context)
        {
            Debug.WriteLine("RecipesController initialized");
        }

        private List<SelectListItem> GetCategorySelectList(int? selectedCategoryId = null)
        {
            Debug.WriteLine("GetCategorySelectList called");
            return _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name,
                Selected = selectedCategoryId.HasValue && c.CategoryId == selectedCategoryId.Value
            }).ToList();
        }

        public IActionResult Create()
        {
            Debug.WriteLine("GET Create action called");
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId?.ToString() ?? "null"}");
            if (!userId.HasValue)
            {
                Debug.WriteLine("Redirecting to Login due to null UserId");
                return RedirectToAction("Login", "Users");
            }

            var viewModel = new CreateRecipeViewModel
            {
                Recipe = new RecipeViewModel(),
                RecipeSteps = new List<RecipeStepViewModel> { new RecipeStepViewModel() },
                Categories = GetCategorySelectList()
            };
            Debug.WriteLine($"CreateRecipeViewModel Version: {viewModel.Version}");

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(CreateRecipeViewModel viewModel, IFormFile image)
        {
            Debug.WriteLine("POST Create action called");
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId?.ToString() ?? "null"}");
            if (!userId.HasValue)
            {
                Debug.WriteLine("Redirecting to Login due to null UserId");
                return RedirectToAction("Login", "Users");
            }

            // Log received data
            Debug.WriteLine($"Received viewModel: {(viewModel == null ? "null" : "not null")}");
            Debug.WriteLine($"CreateRecipeViewModel Version: {(viewModel != null ? viewModel.Version : "null")}");
            if (viewModel != null)
            {
                Debug.WriteLine($"Recipe.Title: {viewModel.Recipe?.Title ?? "null"}");
                Debug.WriteLine($"Recipe.CategoryId: {viewModel.Recipe?.CategoryId?.ToString() ?? "null"}");
                Debug.WriteLine($"Recipe.Description: {viewModel.Recipe?.Description ?? "null"}");
                Debug.WriteLine($"Recipe.Ingredients: {viewModel.Recipe?.Ingredients ?? "null"}");
                Debug.WriteLine($"Recipe.Instructions: {viewModel.Recipe?.Instructions ?? "null"}");
                Debug.WriteLine($"Recipe.ImageUrl: {viewModel.Recipe?.ImageUrl ?? "null"}");
                Debug.WriteLine($"Categories: {(viewModel.Categories != null ? $"Count={viewModel.Categories.Count}" : "null")}");
                Debug.WriteLine($"RecipeSteps Count: {viewModel.RecipeSteps?.Count ?? 0}");
                if (viewModel.RecipeSteps != null)
                {
                    for (int i = 0; i < viewModel.RecipeSteps.Count; i++)
                    {
                        Debug.WriteLine($"Step {i + 1}: Title={viewModel.RecipeSteps[i].Title ?? "null"}, Description={viewModel.RecipeSteps[i].Description ?? "null"}, StepOrder={viewModel.RecipeSteps[i].StepOrder}, ImageUrl={viewModel.RecipeSteps[i].ImageUrl ?? "null"}");
                    }
                }
                Debug.WriteLine($"Image: {(image != null ? image.FileName : "null")}");
                Debug.WriteLine("Form Files:");
                foreach (var file in Request.Form.Files)
                {
                    Debug.WriteLine($"Form File: Name={file.Name}, FileName={file.FileName}");
                }
            }

            // Check if viewModel is null
            if (viewModel == null)
            {
                Debug.WriteLine("Error: viewModel is null");
                ModelState.AddModelError("", "Dữ liệu gửi lên không hợp lệ.");
                viewModel = new CreateRecipeViewModel { Categories = GetCategorySelectList() };
                return View(viewModel);
            }

            // Validate CategoryId
            if (viewModel.Recipe.CategoryId == null || viewModel.Recipe.CategoryId == 0)
            {
                ModelState.AddModelError("Recipe.CategoryId", "Danh mục là bắt buộc.");
                Debug.WriteLine("Validation Error: CategoryId is null or 0");
            }

            // Log ModelState before validation
            Debug.WriteLine("ModelState before validation:");
            foreach (var entry in ModelState)
            {
                Debug.WriteLine($"Key: {entry.Key}, Value: {entry.Value.RawValue ?? "null"}, Errors: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            // Clear specific validation errors
            if (ModelState.ContainsKey("Categories"))
            {
                ModelState["Categories"].Errors.Clear();
                ModelState["Categories"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.ContainsKey("Recipe.ImageUrl"))
            {
                ModelState["Recipe.ImageUrl"].Errors.Clear();
                ModelState["Recipe.ImageUrl"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            for (int i = 0; i < viewModel.RecipeSteps?.Count; i++)
            {
                var key = $"RecipeSteps[{i}].ImageUrl";
                if (ModelState.ContainsKey(key))
                {
                    ModelState[key].Errors.Clear();
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            }

            // Log ModelState after clearing errors
            Debug.WriteLine("ModelState after clearing errors:");
            foreach (var entry in ModelState)
            {
                Debug.WriteLine($"Key: {entry.Key}, Value: {entry.Value.RawValue ?? "null"}, Errors: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            // Check ModelState
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        Debug.WriteLine($"Validation Error for {error.Key}: {err.ErrorMessage}");
                    }
                }
                viewModel.Categories = GetCategorySelectList(viewModel.Recipe.CategoryId);
                return View(viewModel);
            }

            try
            {
                // Validate UserId
                int parsedUserId = userId ?? throw new Exception("Invalid UserId in session");
                Debug.WriteLine($"Validated UserId: {parsedUserId}");
                if (!_context.Users.Any(u => u.UserId == parsedUserId))
                {
                    throw new Exception($"UserId {parsedUserId} does not exist in Users table");
                }

                // Validate CategoryId exists in database
                if (!_context.Categories.Any(c => c.CategoryId == viewModel.Recipe.CategoryId))
                {
                    throw new Exception($"CategoryId {viewModel.Recipe.CategoryId} does not exist in Categories table");
                }
                Debug.WriteLine($"Validated CategoryId: {viewModel.Recipe.CategoryId}");

                // Ensure wwwroot/images directory exists
                var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(imagesDir))
                {
                    Directory.CreateDirectory(imagesDir);
                    Debug.WriteLine($"Created directory: {imagesDir}");
                }

                // Handle image upload
                string imageUrl = null;
                if (image != null && image.Length > 0)
                {
                    if (image.Length > 5 * 1024 * 1024) // Max 5MB
                    {
                        throw new Exception("Recipe image file size exceeds 5MB limit");
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(imagesDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }
                    imageUrl = "/images/" + fileName;
                    Debug.WriteLine($"Saved Recipe Image: {imageUrl}");
                }

                // Check Title length (assume max 255 characters in DB)
                if (viewModel.Recipe.Title != null && viewModel.Recipe.Title.Length > 255)
                {
                    throw new Exception($"Recipe.Title exceeds maximum length of 255 characters: {viewModel.Recipe.Title.Length} characters");
                }

                // Create recipe
                var recipe = new Recipe
                {
                    Title = viewModel.Recipe.Title ?? throw new Exception("Recipe.Title cannot be null"),
                    Description = viewModel.Recipe.Description,
                    Ingredients = viewModel.Recipe.Ingredients,
                    Instructions = viewModel.Recipe.Instructions,
                    CategoryId = viewModel.Recipe.CategoryId ?? throw new Exception("Recipe.CategoryId cannot be null"),
                    UserId = parsedUserId,
                    CreatedAt = DateTime.Now,
                    ImageUrl = imageUrl
                };
                Debug.WriteLine($"Prepared Recipe: Title={recipe.Title} (Length={recipe.Title.Length}), CategoryId={recipe.CategoryId}, UserId={recipe.UserId}, ImageUrl={recipe.ImageUrl ?? "null"}, IsApproved={recipe.IsApproved?.ToString() ?? "null"}");

                _context.Recipes.Add(recipe);
                _context.SaveChanges();
                Debug.WriteLine($"Saved Recipe ID: {recipe.RecipeId}");

                // Handle step images from Request.Form.Files
                int stepOrder = 1;
                for (int i = 0; i < viewModel.RecipeSteps.Count; i++)
                {
                    var step = viewModel.RecipeSteps[i];
                    if (!string.IsNullOrWhiteSpace(step.Title) && !string.IsNullOrWhiteSpace(step.Description))
                    {
                        // Check Title length for RecipeStep (max 200 as per model)
                        if (step.Title != null && step.Title.Length > 200)
                        {
                            throw new Exception($"Step {i + 1} Title exceeds maximum length of 200 characters: {step.Title.Length} characters");
                        }

                        string stepImageUrl = null;
                        var stepImage = Request.Form.Files[$"stepImages[{i}]"];
                        if (stepImage != null && stepImage.Length > 0)
                        {
                            if (stepImage.Length > 5 * 1024 * 1024) // Max 5MB
                            {
                                throw new Exception($"Step {i + 1} image file size exceeds 5MB limit");
                            }
                            var stepFileName = Guid.NewGuid().ToString() + Path.GetExtension(stepImage.FileName);
                            var stepFilePath = Path.Combine(imagesDir, stepFileName);
                            using (var stream = new FileStream(stepFilePath, FileMode.Create))
                            {
                                stepImage.CopyTo(stream);
                            }
                            stepImageUrl = "/images/" + stepFileName;
                            Debug.WriteLine($"Saved Step {i + 1} Image: {stepImageUrl}");
                        }

                        var recipeStep = new RecipeStep
                        {
                            RecipeId = recipe.RecipeId,
                            Title = step.Title ?? throw new Exception($"Step {i + 1} Title cannot be null"),
                            Description = step.Description ?? throw new Exception($"Step {i + 1} Description cannot be null"),
                            ImageUrl = stepImageUrl,
                            StepOrder = stepOrder
                        };
                        Debug.WriteLine($"Prepared Step {stepOrder}: Title={recipeStep.Title} (Length={recipeStep.Title.Length}), Description={recipeStep.Description}, ImageUrl={recipeStep.ImageUrl ?? "null"}");
                        _context.RecipeSteps.Add(recipeStep);
                        stepOrder++;
                    }
                }

                _context.SaveChanges();
                Debug.WriteLine("Database changes saved successfully");
                return RedirectToAction("MyRecipes");
            }
            catch (Exception ex)
            {
                // Log full exception details
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $" | Inner: {innerException.Message}";
                    innerException = innerException.InnerException;
                }
                Debug.WriteLine($"Exception: {errorMessage}\nStackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Lỗi khi tạo công thức: {ex.Message}");
                viewModel.Categories = GetCategorySelectList(viewModel.Recipe.CategoryId);
                return View(viewModel);
            }
        }

        public IActionResult MyRecipes()
        {
            Debug.WriteLine("MyRecipes action called");
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId?.ToString() ?? "null"}");
            if (!userId.HasValue)
            {
                Debug.WriteLine("Redirecting to Login due to null UserId");
                return RedirectToAction("Login", "Users");
            }

            var recipes = _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.UserId == userId.Value)
                .ToList();
            return View(recipes);
        }

        public IActionResult Edit(int id)
        {
            Debug.WriteLine($"Edit action called with id: {id}");
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId?.ToString() ?? "null"}");
            if (!userId.HasValue)
            {
                Debug.WriteLine("Redirecting to Login due to null UserId");
                return RedirectToAction("Login", "Users");
            }

            var recipe = _context.Recipes
                .Include(r => r.Steps)
                .Include(r => r.Category)
                .FirstOrDefault(r => r.RecipeId == id && r.UserId == userId.Value);

            if (recipe == null)
            {
                Debug.WriteLine($"Recipe with id {id} not found");
                return NotFound();
            }

            var viewModel = new CreateRecipeViewModel
            {
                Recipe = new RecipeViewModel
                {
                    RecipeId = recipe.RecipeId,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    CategoryId = recipe.CategoryId,
                    ImageUrl = recipe.ImageUrl
                },
                RecipeSteps = recipe.Steps.OrderBy(s => s.StepOrder).Select(s => new RecipeStepViewModel
                {
                    Title = s.Title,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    StepOrder = s.StepOrder
                }).ToList(),
                Categories = GetCategorySelectList(recipe.CategoryId)
            };
            Debug.WriteLine($"CreateRecipeViewModel Version in Edit: {viewModel.Version}");

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(CreateRecipeViewModel viewModel, IFormFile image)
        {
            Debug.WriteLine("POST Edit action called");
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId?.ToString() ?? "null"}");
            if (!userId.HasValue)
            {
                Debug.WriteLine("Redirecting to Login due to null UserId");
                return RedirectToAction("Login", "Users");
            }

            // Log received data
            Debug.WriteLine($"Received viewModel: {(viewModel == null ? "null" : "not null")}");
            Debug.WriteLine($"CreateRecipeViewModel Version: {(viewModel != null ? viewModel.Version : "null")}");
            if (viewModel != null)
            {
                Debug.WriteLine($"Recipe.RecipeId: {viewModel.Recipe?.RecipeId}");
                Debug.WriteLine($"Recipe.Title: {viewModel.Recipe?.Title ?? "null"}");
                Debug.WriteLine($"Recipe.CategoryId: {viewModel.Recipe?.CategoryId?.ToString() ?? "null"}");
                Debug.WriteLine($"Recipe.ImageUrl: {viewModel.Recipe?.ImageUrl ?? "null"}");
                Debug.WriteLine($"Categories: {(viewModel.Categories != null ? $"Count={viewModel.Categories.Count}" : "null")}");
                Debug.WriteLine($"RecipeSteps Count: {viewModel.RecipeSteps?.Count ?? 0}");
                if (viewModel.RecipeSteps != null)
                {
                    for (int i = 0; i < viewModel.RecipeSteps.Count; i++)
                    {
                        Debug.WriteLine($"Step {i + 1}: Title={viewModel.RecipeSteps[i].Title ?? "null"}, Description={viewModel.RecipeSteps[i].Description ?? "null"}, StepOrder={viewModel.RecipeSteps[i].StepOrder}, ImageUrl={viewModel.RecipeSteps[i].ImageUrl ?? "null"}");
                    }
                }
                Debug.WriteLine($"Image: {(image != null ? image.FileName : "null")}");
                Debug.WriteLine("Form Files:");
                foreach (var file in Request.Form.Files)
                {
                    Debug.WriteLine($"Form File: Name={file.Name}, FileName={file.FileName}");
                }
            }

            // Check if viewModel is null
            if (viewModel == null)
            {
                Debug.WriteLine("Error: viewModel is null in Edit");
                ModelState.AddModelError("", "Dữ liệu gửi lên không hợp lệ.");
                viewModel = new CreateRecipeViewModel { Categories = GetCategorySelectList() };
                return View(viewModel);
            }

            // Validate CategoryId
            if (viewModel.Recipe.CategoryId == null || viewModel.Recipe.CategoryId == 0)
            {
                ModelState.AddModelError("Recipe.CategoryId", "Danh mục là bắt buộc.");
                Debug.WriteLine("Validation Error: CategoryId is null or 0");
            }

            // Log ModelState before validation
            Debug.WriteLine("ModelState before validation:");
            foreach (var entry in ModelState)
            {
                Debug.WriteLine($"Key: {entry.Key}, Value: {entry.Value.RawValue ?? "null"}, Errors: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            // Clear specific validation errors
            if (ModelState.ContainsKey("Categories"))
            {
                ModelState["Categories"].Errors.Clear();
                ModelState["Categories"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.ContainsKey("Recipe.ImageUrl"))
            {
                ModelState["Recipe.ImageUrl"].Errors.Clear();
                ModelState["Recipe.ImageUrl"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.ContainsKey("image")) // Clear validation for image field in Edit action
            {
                ModelState["image"].Errors.Clear();
                ModelState["image"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            for (int i = 0; i < viewModel.RecipeSteps?.Count; i++)
            {
                var key = $"RecipeSteps[{i}].ImageUrl";
                if (ModelState.ContainsKey(key))
                {
                    ModelState[key].Errors.Clear();
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            }

            // Log ModelState after clearing errors
            Debug.WriteLine("ModelState after clearing errors:");
            foreach (var entry in ModelState)
            {
                Debug.WriteLine($"Key: {entry.Key}, Value: {entry.Value.RawValue ?? "null"}, Errors: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            // Check ModelState
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        Debug.WriteLine($"Validation Error for {error.Key}: {err.ErrorMessage}");
                    }
                }
                viewModel.Categories = GetCategorySelectList(viewModel.Recipe.CategoryId);
                return View(viewModel);
            }

            try
            {
                // Validate UserId
                int parsedUserId = userId ?? throw new Exception("Invalid UserId in session");
                Debug.WriteLine($"Validated UserId: {parsedUserId}");
                if (!_context.Users.Any(u => u.UserId == parsedUserId))
                {
                    throw new Exception($"UserId {parsedUserId} does not exist in Users table");
                }

                // Validate CategoryId exists in database
                if (!_context.Categories.Any(c => c.CategoryId == viewModel.Recipe.CategoryId))
                {
                    throw new Exception($"CategoryId {viewModel.Recipe.CategoryId} does not exist in Categories table");
                }
                Debug.WriteLine($"Validated CategoryId: {viewModel.Recipe.CategoryId}");

                // Find the existing recipe
                var recipe = _context.Recipes
                    .Include(r => r.Steps)
                    .FirstOrDefault(r => r.RecipeId == viewModel.Recipe.RecipeId && r.UserId == parsedUserId);
                if (recipe == null)
                {
                    Debug.WriteLine($"Recipe with id {viewModel.Recipe.RecipeId} not found");
                    return NotFound();
                }

                // Ensure wwwroot/images directory exists
                var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(imagesDir))
                {
                    Directory.CreateDirectory(imagesDir);
                    Debug.WriteLine($"Created directory: {imagesDir}");
                }

                // Handle image upload (only if a new image is uploaded)
                string imageUrl = recipe.ImageUrl; // Keep existing ImageUrl by default
                if (image != null && image.Length > 0)
                {
                    if (image.Length > 5 * 1024 * 1024) // Max 5MB
                    {
                        throw new Exception("Recipe image file size exceeds 5MB limit");
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(imagesDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }
                    imageUrl = "/images/" + fileName;
                    Debug.WriteLine($"Saved Recipe Image: {imageUrl}");

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(recipe.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", recipe.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                            Debug.WriteLine($"Deleted old Recipe Image: {recipe.ImageUrl}");
                        }
                    }
                }

                // Check Title length (assume max 255 characters in DB)
                if (viewModel.Recipe.Title != null && viewModel.Recipe.Title.Length > 255)
                {
                    throw new Exception($"Recipe.Title exceeds maximum length of 255 characters: {viewModel.Recipe.Title.Length} characters");
                }

                // Update recipe fields
                recipe.Title = viewModel.Recipe.Title ?? throw new Exception("Recipe.Title cannot be null");
                recipe.Description = viewModel.Recipe.Description;
                recipe.Ingredients = viewModel.Recipe.Ingredients;
                recipe.Instructions = viewModel.Recipe.Instructions;
                recipe.CategoryId = viewModel.Recipe.CategoryId ?? throw new Exception("Recipe.CategoryId cannot be null");
                recipe.ImageUrl = imageUrl;
                recipe.UpdatedAt = DateTime.Now;
                // Keep CreatedAt and IsApproved unchanged
                Debug.WriteLine($"Updated Recipe: Title={recipe.Title} (Length={recipe.Title.Length}), CategoryId={recipe.CategoryId}, UserId={recipe.UserId}, ImageUrl={recipe.ImageUrl ?? "null"}, IsApproved={recipe.IsApproved?.ToString() ?? "null"}");

                // Remove existing steps
                if (recipe.Steps != null && recipe.Steps.Any())
                {
                    foreach (var step in recipe.Steps)
                    {
                        if (!string.IsNullOrEmpty(step.ImageUrl))
                        {
                            var stepImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", step.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(stepImagePath))
                            {
                                System.IO.File.Delete(stepImagePath);
                                Debug.WriteLine($"Deleted Step Image: {step.ImageUrl}");
                            }
                        }
                    }
                    _context.RecipeSteps.RemoveRange(recipe.Steps);
                    Debug.WriteLine($"Removed {recipe.Steps.Count} existing steps for Recipe ID: {recipe.RecipeId}");
                }

                // Add new steps
                int stepOrder = 1;
                for (int i = 0; i < viewModel.RecipeSteps.Count; i++)
                {
                    var step = viewModel.RecipeSteps[i];
                    if (!string.IsNullOrWhiteSpace(step.Title) && !string.IsNullOrWhiteSpace(step.Description))
                    {
                        // Check Title length for RecipeStep (max 200 as per model)
                        if (step.Title != null && step.Title.Length > 200)
                        {
                            throw new Exception($"Step {i + 1} Title exceeds maximum length of 200 characters: {step.Title.Length} characters");
                        }

                        // Handle step image (keep existing ImageUrl if no new image is uploaded)
                        string stepImageUrl = step.ImageUrl; // Keep existing ImageUrl by default
                        var stepImage = Request.Form.Files[$"stepImages[{i}]"];
                        if (stepImage != null && stepImage.Length > 0)
                        {
                            if (stepImage.Length > 5 * 1024 * 1024) // Max 5MB
                            {
                                throw new Exception($"Step {i + 1} image file size exceeds 5MB limit");
                            }
                            var stepFileName = Guid.NewGuid().ToString() + Path.GetExtension(stepImage.FileName);
                            var stepFilePath = Path.Combine(imagesDir, stepFileName);
                            using (var stream = new FileStream(stepFilePath, FileMode.Create))
                            {
                                stepImage.CopyTo(stream);
                            }
                            stepImageUrl = "/images/" + stepFileName;
                            Debug.WriteLine($"Saved Step {i + 1} Image: {stepImageUrl}");

                            // Delete old step image if exists
                            if (!string.IsNullOrEmpty(step.ImageUrl))
                            {
                                var oldStepImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", step.ImageUrl.TrimStart('/'));
                                if (System.IO.File.Exists(oldStepImagePath))
                                {
                                    System.IO.File.Delete(oldStepImagePath);
                                    Debug.WriteLine($"Deleted old Step Image: {step.ImageUrl}");
                                }
                            }
                        }

                        var recipeStep = new RecipeStep
                        {
                            RecipeId = recipe.RecipeId,
                            Title = step.Title ?? throw new Exception($"Step {i + 1} Title cannot be null"),
                            Description = step.Description ?? throw new Exception($"Step {i + 1} Description cannot be null"),
                            ImageUrl = stepImageUrl,
                            StepOrder = stepOrder
                        };
                        Debug.WriteLine($"Prepared Step {stepOrder}: Title={recipeStep.Title} (Length={recipeStep.Title.Length}), Description={recipeStep.Description}, ImageUrl={recipeStep.ImageUrl ?? "null"}");
                        _context.RecipeSteps.Add(recipeStep);
                        stepOrder++;
                    }
                }

                _context.SaveChanges();
                Debug.WriteLine("Database changes saved successfully");
                return RedirectToAction("MyRecipes");
            }
            catch (Exception ex)
            {
                // Log full exception details
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $" | Inner: {innerException.Message}";
                    innerException = innerException.InnerException;
                }
                Debug.WriteLine($"Exception: {errorMessage}\nStackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Lỗi khi chỉnh sửa công thức: {ex.Message}");
                viewModel.Categories = GetCategorySelectList(viewModel.Recipe.CategoryId);
                return View(viewModel);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Debug.WriteLine($"Delete action called with id: {id}");
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId?.ToString() ?? "null"}");
            if (!userId.HasValue)
            {
                Debug.WriteLine("Redirecting to Login due to null UserId");
                return RedirectToAction("Login", "Users");
            }

            var recipe = _context.Recipes
                .Include(r => r.Steps) // Include related RecipeSteps
                .FirstOrDefault(r => r.RecipeId == id && r.UserId == userId.Value);
            if (recipe == null)
            {
                Debug.WriteLine($"Recipe with id {id} not found");
                return NotFound();
            }

            // Delete associated images
            if (!string.IsNullOrEmpty(recipe.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", recipe.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    Debug.WriteLine($"Deleted Recipe Image: {recipe.ImageUrl}");
                }
            }

            // Delete associated RecipeSteps and their images
            if (recipe.Steps != null && recipe.Steps.Any())
            {
                foreach (var step in recipe.Steps)
                {
                    if (!string.IsNullOrEmpty(step.ImageUrl))
                    {
                        var stepImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", step.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(stepImagePath))
                        {
                            System.IO.File.Delete(stepImagePath);
                            Debug.WriteLine($"Deleted Step Image: {step.ImageUrl}");
                        }
                    }
                }
                _context.RecipeSteps.RemoveRange(recipe.Steps);
                Debug.WriteLine($"Removed {recipe.Steps.Count} steps for Recipe ID: {id}");
            }

            // Delete the recipe
            _context.Recipes.Remove(recipe);
            _context.SaveChanges();
            Debug.WriteLine($"Deleted Recipe ID: {id}");
            return RedirectToAction("MyRecipes");
        }
    }
}