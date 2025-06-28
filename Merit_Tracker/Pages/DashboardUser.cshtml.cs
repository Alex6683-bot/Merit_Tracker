using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Merit_Tracker.Pages
{
    public class DashboardUserModel : PageModel, IUserPage
    {
		private readonly AppDatabaseContext _dbContext;
		public UserModel currentUser { get; set; }
		public DashboardUserModel(AppDatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}
		public IActionResult OnGet()
		{
			HttpContext.Session.TryGetValue("UserID", out byte[] id);
			HttpContext.Session.TryGetValue("Role", out byte[] role);


			if (id == null || System.Text.Encoding.Default.GetString(role) != "User") return RedirectToPage("Index");

            int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
            currentUser = _dbContext.Users.Where(u => u.ID == userID).ToList().FirstOrDefault();

            return Page();
		}
	}
}
