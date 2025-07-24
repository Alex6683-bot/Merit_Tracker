using Merit_Tracker.Database;
using Merit_Tracker.Models;

namespace Merit_Tracker.Interfaces
{
	public interface IUserService
	{
		public Task<UserModel> GetCurrentUserAsync(HttpContext httpContext, AppDatabaseContext dbContext);
		public Task<UserModel> RegisterUserSessionAsync(HttpContext httpContext, AppDatabaseContext dbContext, string userName, string password);
	}
}
