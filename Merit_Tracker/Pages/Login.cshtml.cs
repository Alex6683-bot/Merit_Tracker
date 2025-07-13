using Merit_Tracker.Database;
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
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly AppDatabaseContext dbContext;

        public LoginModel(AppDatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            SHA256 sha256 = SHA256.Create();
            string hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(Password)));
            sha256.Dispose();

            UserModel user = dbContext.Users.Where(u => u.UserName == Username).ToList().FirstOrDefault();

            if (user != null && user.Password == hashedPassword)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetInt32("UserID", user.ID);
                HttpContext.Session.SetInt32("Role", (int)user.Role);

                if (user.Role == UserRole.Admin || user.Role == UserRole.Teacher)
                    return new JsonResult(new { url = Url.Page("Dashboard") });
            }

            return new ContentResult()
            {
                Content = "Error Login",
                ContentType = "text/plain",
                StatusCode = 400
            };
        }
    }
}
