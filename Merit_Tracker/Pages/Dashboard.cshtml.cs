using Merit_Tracker.Database;
using Merit_Tracker.Helpers;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    public class DashboardModel : PageModel
    {
        public UserModel CurrentUser { get; set; }
        public readonly AppDatabaseContext dbContext;

        public DashboardModel(AppDatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult OnGet()
        {
			var user = HelperMethods.GetCurrentUser(HttpContext, dbContext);
			if (user == null) return RedirectToPage("/Login"); // Return to login if current user is null or invalid

            CurrentUser = user;

            return Page();
        }

        public IActionResult OnGetRedirect(int databaseID)
        {

            return RedirectToPage("/DatabaseEditor", new { databaseID });
            
        }
    }
}
