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
        private readonly IUserService userService;

        public DashboardModel(AppDatabaseContext dbContext, IUserService userService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
			var user = await userService.GetCurrentUserAsync(HttpContext, dbContext);
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
