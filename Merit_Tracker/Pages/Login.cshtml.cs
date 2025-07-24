using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
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
        private readonly IUserService userService;

        public LoginModel(AppDatabaseContext dbContext, IUserService userServie)
        {
            this.dbContext = dbContext;
            this.userService = userServie;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userService.RegisterUserSessionAsync(HttpContext, dbContext, Username, Password); // Get current user after registering to session with user service

            if (user != null)
            {
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
