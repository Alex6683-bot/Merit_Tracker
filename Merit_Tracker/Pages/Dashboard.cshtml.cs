using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    public class DashboardModel : PageModel
    {
        public UserModel currentUser { get; set; }
        public readonly AppDatabaseContext dbContext;

        public DashboardModel(AppDatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult OnGet()
        {
            HttpContext.Session.TryGetValue("UserID", out byte[] id);
            HttpContext.Session.TryGetValue("Role", out byte[] role);

            if (id == null || role == null) return RedirectToPage("Login");

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(id);
                Array.Reverse(role);
            }

            int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
            currentUser = dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();

            return Page();
        }

        public IActionResult OnGetRedirect(int databaseID)
        {

            return RedirectToPage("/DatabaseEditor", new { databaseID });
            
        }
    }
}
