using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Merit_Tracker.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly AppDatabaseContext _dbContext;

        public IndexModel(ILogger<IndexModel> logger, AppDatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            SHA256 sha256 = SHA256.Create();
            string hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(Password)));
            sha256.Dispose();

            UserModel user = _dbContext.Users.Where(u => u.UserName == Username).ToList().FirstOrDefault();

            if (user != null && user.Password == hashedPassword)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetInt32("UserID", user.ID);
                HttpContext.Session.SetString("Role", user.Role);

                if (user.Role == "Admin")
                    return RedirectToPage("DashboardAdmin");
                else if (user.Role == "Teacher")
                    return RedirectToPage("DashboardUser");
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
