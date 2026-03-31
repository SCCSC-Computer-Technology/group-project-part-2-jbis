using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using WorldWideSportsApp.Helpers;

namespace WorldWideSportsApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connString;
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
            _connString = config.GetConnectionString("DefaultConnection")!;
        }

        //shows the login page
        [HttpGet]
        public IActionResult Login() => View();

        //Handles form submission and verifies the request came from our own form and not an outside attack
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            // if txtboxes empty/null/whitespace, show error
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            using var conn = new SqlConnection(_connString);
            conn.Open();

            //the clean up method to delete any unverified users whose codes have expired
            CleanupExpiredUnverifiedUsers(conn);

            //checks if the username exists and pulls the password hash and verification status for that user
            var cmd = new SqlCommand(
                "SELECT UserId, PasswordHash, verified FROM Users WHERE Username = @Username", conn);
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = cmd.ExecuteReader();

            //if no user found with that username, show error
            if (!reader.Read())
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            int userId = reader.GetInt32(0);
            string storedHash = reader.GetString(1);
            bool isVerified = reader.GetBoolean(2);

            if (!PasswordHelper.VerifyPassword(password, storedHash))
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            if (!isVerified)
            {
                // Store userId so Verify page knows who to verify
                HttpContext.Session.SetInt32("PendingVerifyUserId", userId);
                HttpContext.Session.SetString("PendingVerifyUsername", username);
                return RedirectToAction("Verify");
            }

            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index", "Home");
        }

        //shows the registration page
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword, string email)
        {
            // if txtboxes empty/null/whitespace, show error
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }
            //if passwords don't match, show error
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            using var conn = new SqlConnection(_connString);
            conn.Open();

            //the clean up method to delete any unverified users whose codes have expired
            CleanupExpiredUnverifiedUsers(conn);

            var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username", conn);
            checkCmd.Parameters.AddWithValue("@Username", username);
            if ((int)checkCmd.ExecuteScalar() > 0)
            {
                ViewBag.Error = "Username already taken.";
                return View();
            }

            // Generate 6-digit code
            string code = new Random().Next(100000, 999999).ToString();
            DateTime expires = DateTime.UtcNow.AddMinutes(15);
            string hash = PasswordHelper.HashPassword(password);

            //posting the new user with the code and expiration time to the database
            var insertCmd = new SqlCommand(@"
                INSERT INTO Users (Username, PasswordHash, email, verified, verification_code, verification_expires_at)
                OUTPUT INSERTED.UserId
                VALUES (@Username, @Hash, @Email, 0, @Code, @Expires)", conn);
            insertCmd.Parameters.AddWithValue("@Username", username);
            insertCmd.Parameters.AddWithValue("@Hash", hash);
            insertCmd.Parameters.AddWithValue("@Email", email);
            insertCmd.Parameters.AddWithValue("@Code", code);
            insertCmd.Parameters.AddWithValue("@Expires", expires);

            int newUserId = (int)insertCmd.ExecuteScalar();

            //send verification email
            await EmailHelper.SendVerificationEmailAsync(_config, email, code);

            //store temp for the user session
            HttpContext.Session.SetInt32("PendingVerifyUserId", newUserId);
            HttpContext.Session.SetString("PendingVerifyUsername", username);

            return RedirectToAction("Verify");
        }

        //shows the verify page where user can enter the code they received in their email
        [HttpGet]
        public IActionResult Verify()
        {
            if (HttpContext.Session.GetInt32("PendingVerifyUserId") == null)
                return RedirectToAction("Login");

            return View();
        }


        //verify action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(string code)
        {
            //pull the pending user info from session
            int? userId = HttpContext.Session.GetInt32("PendingVerifyUserId");
            string? username = HttpContext.Session.GetString("PendingVerifyUsername");

            //if the session timed out then redirect to login
            if (userId == null)
                return RedirectToAction("Login");

            using var conn = new SqlConnection(_connString);
            conn.Open();

            //get the stored code and expiration time for this user
            var cmd = new SqlCommand(
                "SELECT verification_code, verification_expires_at FROM Users WHERE UserId = @UserId", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                ViewBag.Error = "Something went wrong. Please register again.";
                return View();
            }

            string storedCode = reader.GetString(0);
            DateTime expires = reader.GetDateTime(1);
            reader.Close();

            //if the code is expired after 15 mins, show error
            if (DateTime.UtcNow > expires)
            {
                ViewBag.Error = "Code expired. Please register again.";
                return View();
            }

            //if the code doesn't match, show error
            if (code != storedCode)
            {
                ViewBag.Error = "Incorrect code. Please try again.";
                return View();
            }

            // check why we're verifying and route accordingly
            string? purpose = HttpContext.Session.GetString("VerifyPurpose");

            if (purpose == "reset")
            {
                // for password reset — don't mark as verified, just confirm the code was correct
                var resetUpdateCmd = new SqlCommand(
                    "UPDATE Users SET verification_code = NULL, verification_expires_at = NULL WHERE UserId = @UserId", conn);
                resetUpdateCmd.Parameters.AddWithValue("@UserId", userId);
                resetUpdateCmd.ExecuteNonQuery();

                // flag that they passed the code check and send to reset page
                HttpContext.Session.SetString("PasswordResetVerified", "true");
                HttpContext.Session.Remove("PendingVerifyUsername");
                return RedirectToAction("ResetPassword");
            }
            else
            {
                // normal registration flow — mark as verified and send to login
                var registerUpdateCmd = new SqlCommand(
                    "UPDATE Users SET verified = 1, verification_code = NULL, verification_expires_at = NULL WHERE UserId = @UserId", conn);
                registerUpdateCmd.Parameters.AddWithValue("@UserId", userId);
                registerUpdateCmd.ExecuteNonQuery();

                HttpContext.Session.Remove("PendingVerifyUserId");
                HttpContext.Session.Remove("PendingVerifyUsername");
                HttpContext.Session.Remove("VerifyPurpose");
                return RedirectToAction("Login");
            }
        }

        //shows the email page when forgot password is clicked
        [HttpGet]
        public IActionResult ForgotPassword() => View("Email");

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //deletes any accounts that were never verified and whose code has expired
        private void CleanupExpiredUnverifiedUsers(SqlConnection conn)
        {
            var cmd = new SqlCommand(@"
            DELETE FROM Users 
            WHERE verified = 0 
            AND verification_expires_at < @Now", conn);
            cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);
            cmd.ExecuteNonQuery();
        }

        //this will run when the email form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            //if the email is empty/null/whitespace, show error
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please enter your email.";
                return View("Email");
            }

            using var conn = new SqlConnection(_connString);
            conn.Open();

            //look up user by email
            var cmd = new SqlCommand(
                "SELECT UserId FROM Users WHERE email = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);
            var result = cmd.ExecuteScalar();

            //this will not reveal whether the email exists in our system or not, but if it does we will generate a code and send the email
            if (result != null)
            {
                int userId = (int)result;

                //generate new code and expiration time
                string code = new Random().Next(100000, 999999).ToString();
                DateTime expires = DateTime.UtcNow.AddMinutes(15);

                var updateCmd = new SqlCommand(@"
                UPDATE Users 
                SET verification_code = @Code, verification_expires_at = @Expires 
                WHERE UserId = @UserId", conn);
                updateCmd.Parameters.AddWithValue("@Code", code);
                updateCmd.Parameters.AddWithValue("@Expires", expires);
                updateCmd.Parameters.AddWithValue("@UserId", userId);
                updateCmd.ExecuteNonQuery();

                //this will send the code to their email
                await EmailHelper.SendVerificationEmailAsync(_config, email, code);

                //i needed to make sure the verify page knew this was for a password reset and not a new registration, so I set a different session variable for the userId and a purpose
                HttpContext.Session.SetInt32("PendingVerifyUserId", userId);
                HttpContext.Session.SetString("VerifyPurpose", "reset");
            }

            // Always redirect to verify so we don't leak if email exists
            return RedirectToAction("Verify");
        }

        // Shows the reset password form — only accessible after code verified
        [HttpGet]
        public IActionResult ResetPassword()
        {
            // Must have completed verify step first
            if (HttpContext.Session.GetString("PasswordResetVerified") != "true")
                return RedirectToAction("Login");

            return View("Forgot");
        }

        // Runs when the new password form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string password, string confirmPassword)
        {
            // Must have completed verify step first
            if (HttpContext.Session.GetString("PasswordResetVerified") != "true")
                return RedirectToAction("Login");

            //if the passwords are empty/null/whitespace, show error
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View("Forgot");
            }

            //if the passwords don't match, show error
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View("Forgot");
            }

            int? userId = HttpContext.Session.GetInt32("PendingVerifyUserId");

            if (userId == null)
                return RedirectToAction("Login");

            // Hash the new password and update the database
            string hash = PasswordHelper.HashPassword(password);

            using var conn = new SqlConnection(_connString);
            conn.Open();

            var cmd = new SqlCommand(@"
            UPDATE Users 
            SET PasswordHash = @Hash, 
            verification_code = NULL, 
            verification_expires_at = NULL 
            WHERE UserId = @UserId", conn);
            cmd.Parameters.AddWithValue("@Hash", hash);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.ExecuteNonQuery();

            //clear all reset session data
            HttpContext.Session.Remove("PendingVerifyUserId");
            HttpContext.Session.Remove("PasswordResetVerified");
            HttpContext.Session.Remove("VerifyPurpose");

            return RedirectToAction("Login");
        }
    }
}