using Microsoft.AspNetCore.Mvc;
using CQCDMS.Models;
using CQCDMS.Data;
using System.Security.Cryptography;
using System.Text;

namespace CQCDMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly DmsDbContext _context;

        public LoginController(DmsDbContext context)
        {
            _context = context;
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // GET: Login
        public IActionResult Index()
        {
            // If user is already logged in, redirect to Home
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User user)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(user.Password))
            {
                // First check if username exists
                var userExists = _context.Users
                    .FirstOrDefault(u => u.Username == user.Username);

                if (userExists == null)
                {
                    // Username doesn't exist
                    ModelState.AddModelError("Username", "اسم المستخدم غير صحيح"); // Invalid username
                }
                else
                {
                    // Username exists, check password
                    string hashedPassword = HashPassword(user.Password);
                    
                    if (userExists.Password != hashedPassword)
                    {
                        // Password is wrong
                        ModelState.AddModelError("Password", "كلمة المرور غير صحيحة"); // Invalid password
                    }
                    else
                    {
                        // Login successful - set session and redirect to home
                        HttpContext.Session.SetString("Username", user.Username ?? "");
                        HttpContext.Session.SetString("IsLoggedIn", "true");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(user);
        }

        // Logout action
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        // Helper method to add a user with hashed password (for testing/seeding)
        public IActionResult AddTestUser(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var user = new User
            {
                Username = username,
                Password = hashedPassword
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Json(new { success = true, message = "User added successfully" });
        }
    }
}
