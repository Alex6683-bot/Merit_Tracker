using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Merit_Tracker.Pages
{
    public class LoginModel : PageModel
    {
        // Username input from login form
        [BindProperty]
        public string Username { get; set; }

        // Password input from login form
        [BindProperty]
        public string Password { get; set; }

        // Logger (currently not used in logic but could be used for debugging/errors)
        private readonly ILogger<IndexModel> _logger;

        // Database context for accessing the user data
        private readonly AppDatabaseContext dbContext;

        // User service to handle session and login validation
        private readonly IUserService userService;

        // Constructor dependency injection of dependencies (db and user service)
        public LoginModel(AppDatabaseContext dbContext, IUserService userServie)
        {
            this.dbContext = dbContext;
            this.userService = userServie;
        }

        // Called when the page is first accessed (GET request)
        public void OnGet()
        {
        }

        // Handles login form submission (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if both username and password are provided
            if (string.IsNullOrEmpty(Username) == false && string.IsNullOrEmpty(Password) == false)
            {
                // Attempt to authenticate and start a user session
                var user = await userService.RegisterUserSessionAsync(HttpContext, dbContext, Username, Password);

                // If user exists and is either admin or teacher, redirect to dashboard
                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Teacher)
                        return new JsonResult(new { url = Url.Page("Dashboard") });
                }
            }

            // If login fails, return an error response
            return new ContentResult()
            {
                Content = "Error Login",
                ContentType = "text/plain",
                StatusCode = 400
            };
        }
    }
}
