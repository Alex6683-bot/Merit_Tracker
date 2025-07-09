using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    public class DashboardAdminModel : PageModel, IUserPage
    {
        public UserModel currentUser { get; set; }
        private readonly AppDatabaseContext _dbContext;

        public DashboardAdminModel(AppDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult OnGet()
        {
            HttpContext.Session.TryGetValue("UserID", out byte[] id);
            HttpContext.Session.TryGetValue("Role", out byte[] role);

            if (id == null || System.Text.Encoding.Default.GetString(role) != "Admin") return RedirectToPage("Index");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(id);

            int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
            currentUser = _dbContext.Users.Where(u => u.ID == userID).ToList().FirstOrDefault();
            return Page();
        }
    }
}
