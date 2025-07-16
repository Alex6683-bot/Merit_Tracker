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


        public IActionResult OnGet()
        {
            if (HttpContext.Session != null || HttpContext.Session.Keys.Count() > 0)
            {
                HttpContext.Session.TryGetValue("UserID", out byte[] id);
                HttpContext.Session.TryGetValue("Role", out byte[] role);

                if (id == null || role == null) return RedirectToPage("Login");

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(id);
                    Array.Reverse(role);
                }

                UserRole roleConverted = (UserRole)BitConverter.ToInt32(role);

                if (roleConverted == UserRole.Admin || roleConverted == UserRole.Teacher) return RedirectToPage("Dashboard");

            }
            return RedirectToPage("Login");
        }

        public void OnPost()
        {

            
        }
    }
}
