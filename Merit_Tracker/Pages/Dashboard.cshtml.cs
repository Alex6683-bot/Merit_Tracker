using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    public class DashboardModel : PageModel, IUserPage
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

            if (id == null) return RedirectToPage("Login");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(id);
                Array.Reverse(role);

            int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
            currentUser = dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();

            //dbContext.Merits.Add(new MeritModel()
            //{
            //    DatabaseID = 1,
            //    HousePoints = 3,
            //    StudentName = "Ben",
            //    Value = MeritValueType.Resilience,
            //    DateOfIssue = new DateTime(2005, 5, 6)
            //});

            //var m = dbContext.Merits.Where(m => m.DatabaseID == 1).FirstOrDefault();
            dbContext.SaveChanges();
            return Page();
        }
    }
}
