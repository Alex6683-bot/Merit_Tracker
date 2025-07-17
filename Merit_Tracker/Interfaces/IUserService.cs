using Merit_Tracker.Models;

namespace Merit_Tracker.Interfaces
{
	public interface IUserService
	{
		public UserModel CurrentUser { get; set; }
		public UserModel GetCurrentUser(HttpContext httpContext);
		public UserModel RegisterUserSession(HttpContext httpContext);
	}
}
