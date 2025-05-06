using Microsoft.AspNetCore.Mvc;
using WebShareRecipes.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace WebShareRecipes.Controllers
{
    public class UsersController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context) : base(context)
        {
            _context = context;
            Debug.WriteLine("UsersController initialized");
        }

        // GET: /Users/Register
        public IActionResult Register()
        {
            Debug.WriteLine("GET Register action called");
            return View();
        }

        // POST: /Users/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            Debug.WriteLine("POST Register action called");
            Debug.WriteLine($"Register model: Username={model?.Username}, Email={model?.Email}");

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == model.Username || u.Email == model.Email);
                if (existingUser != null)
                {
                    Debug.WriteLine("Error: Username or Email already exists");
                    ModelState.AddModelError("", "Tên người dùng hoặc email đã tồn tại.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    IsAdmin = false,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                Debug.WriteLine($"User registered: UserId={user.UserId}, Username={user.Username}");

                return RedirectToAction("Login");
            }

            Debug.WriteLine("ModelState is invalid for Register");
            return View(model);
        }

        // GET: /Users/Login
        [HttpGet]
        public IActionResult Login()
        {
            Debug.WriteLine("GET Login action called");
            return View();
        }

        // POST: /Users/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            Debug.WriteLine("POST Login action called");
            Debug.WriteLine($"Login model: Username={model?.Username}");

            if (ModelState.IsValid)
            {
                var passwordHash = HashPassword(model.Password);
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.PasswordHash == passwordHash);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                    Debug.WriteLine($"Login successful: UserId={user.UserId}, Username={user.Username}");
                    Debug.WriteLine($"Session set: UserId={HttpContext.Session.GetInt32("UserId")}, Username={HttpContext.Session.GetString("Username")}");
                    return RedirectToAction("Index", "Home");
                }

                Debug.WriteLine("Login failed: Invalid username or password");
                ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ.");
            }
            else
            {
                Debug.WriteLine("ModelState is invalid for Login");
            }

            return View(model);
        }

        // GET: /Users/Logout
        public IActionResult Logout()
        {
            Debug.WriteLine("Logout action called");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
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