using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;

namespace Merit_Tracker.Services
{
	public class UserService : IUserService
	{
		public UserModel CurrentUser { get; set; }
		private AppDatabaseContext dbContext;

		public UserService(AppDatabaseContext dbContext)
		{
			this.dbContext = dbContext;
		}
		 
		public UserModel GetCurrentUser(HttpContext httpContext)
		{
			return null;
		}

		public UserModel RegisterUserSession(HttpContext httpContext)
		{
			return null;
		}
	}
}
