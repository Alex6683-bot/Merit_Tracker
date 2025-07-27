using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    public class LogoutModel : PageModel
    {
        [IgnoreAntiforgeryToken]
        public IActionResult OnPost()
        {
            if (HttpContext.Session != null || HttpContext.Session.Keys.Count() > 0)
            {
                HttpContext.Session.Clear();
            }
            return RedirectToPage("/Index");
        }
    }
}
