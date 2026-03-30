using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using WorldWideSportsApp.Helpers;
using static System.Reflection.Metadata.BlobBuilder;

namespace WorldWideSportsApp.Controllers
{

    // [HttpPost] handles form submissions and ensures this only fires when the form is submitted,
    // [ValidateAntiForgeryToken] verifies the request came from our own form and not an outside attack
    public class AccountController : Controller
    {
        private readonly string _connString;

        // Pulls the connection string from appsettings.json
        public AccountController(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public IActionResult Login() => View();

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            using var conn = new SqlConnection(_connString);
            conn.Open();

            // @Username parameter prevents SQL injection
            var cmd = new SqlCommand(
                "SELECT UserId, PasswordHash FROM Users WHERE Username = @Username",
                conn);
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = cmd.ExecuteReader();

            // If no row found, username doesn't exist
            if (!reader.Read())
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            int userId = reader.GetInt32(0);
            string storedHash = reader.GetString(1);

            // Re-hashes the entered password and compares it to the stored hash
            if (!PasswordHelper.VerifyPassword(password, storedHash))
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Store the logged in user's info in session for use across pages
            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetString("Username", username);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        //Handles form submission and blocks outside attacks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            using var conn = new SqlConnection(_connString);
            conn.Open();

            // Check if username is already taken before inserting
            var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username", conn);
            checkCmd.Parameters.AddWithValue("@Username", username);
            int exists = (int)checkCmd.ExecuteScalar();

            if (exists > 0)
            {
                ViewBag.Error = "Username already taken.";
                return View();
            }

            // Salt is embedded inside the hash — no separate Salt column needed
            string hash = PasswordHelper.HashPassword(password);

            var insertCmd = new SqlCommand(
                "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @Hash)",
                conn);
            insertCmd.Parameters.AddWithValue("@Username", username);
            insertCmd.Parameters.AddWithValue("@Hash", hash);
            insertCmd.ExecuteNonQuery();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // Clears the session and sends user back to login
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}