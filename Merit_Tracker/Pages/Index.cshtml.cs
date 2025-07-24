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
		private readonly AppDatabaseContext dbContext;
		private readonly IUserService userService;

		public IndexModel(AppDatabaseContext dbContext, IUserService userServie)
		{
			this.dbContext = dbContext;
			this.userService = userServie;
		}
		public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await userService.GetCurrentUserAsync(HttpContext, dbContext);
            if (currentUser != null)
            {
                if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Teacher)
                {
                    return RedirectToPage("Dashboard");
                }
                else return RedirectToPage("Login");
            }
            return RedirectToPage("Login");
        }

        public void OnPost()
        {

            
        }
    }
}
