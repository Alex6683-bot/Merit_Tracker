using Merit_Tracker.Database;
using Merit_Tracker.Helpers;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    // Page model for the Dashboard page, responsible for showing the user's dashboard after login
    public class DashboardModel : PageModel
    {
        // Stores the currently logged-in user's data
        public UserModel CurrentUser { get; set; }

        // Reference to the application's database context for interacting with the database
        public readonly AppDatabaseContext dbContext;

        // Service used to retrieve the current user based on the session or cookie data
        private readonly IUserService userService;

        // Constructor for dependency injecting the database context and user service
        public DashboardModel(AppDatabaseContext dbContext, IUserService userService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
        }

        // Handles GET requests to the dashboard page
        public async Task<IActionResult> OnGetAsync()
        {
            // Attempt to retrieve the current user from session/cookie data
            var user = await userService.GetCurrentUserAsync(HttpContext, dbContext);

            // If user is not found or not logged in, redirect to the login page
            if (user == null) return RedirectToPage("/Login");

            // Set the current user to be used in the page view
            CurrentUser = user;

            // Return the current page with user data
            return Page();
        }

        // Handles redirection to the Database Editor page for a specific database ID
        public IActionResult OnGetRedirect(int databaseID)
        {
            return RedirectToPage("/DatabaseEditor", new { databaseID });
        }
    }
}
