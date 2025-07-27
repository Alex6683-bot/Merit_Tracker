using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Merit_Tracker.Interfaces;

namespace Merit_Tracker.Pages
{
    public class IndexModel : PageModel
    {
        // Database context for querying users
        private readonly AppDatabaseContext dbContext;

        // Service for user-related logic (e.g., getting current logged-in user)
        private readonly IUserService userService;

        public IndexModel(AppDatabaseContext dbContext, IUserService userServie)
        {
            this.dbContext = dbContext;
            this.userService = userServie;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Attempt to fetch the current logged-in user via cookies/session
            var currentUser = await userService.GetCurrentUserAsync(HttpContext, dbContext);

            // Redirect based on role
            if (currentUser != null)
            {
                if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Teacher)
                {
                    return RedirectToPage("Dashboard");
                }
                else return RedirectToPage("Login"); // User exists but isn't valid (e.g., not an Admin/Teacher)
            }

            // If no user is logged in, redirect to login
            return RedirectToPage("Login");
        }
    }
}
