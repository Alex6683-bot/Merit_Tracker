using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace Merit_Tracker.Interfaces
{
    // Interface used to implement servies that handle user sessions
    public interface IUserService
	{
		public Task<UserModel> GetCurrentUserAsync(HttpContext httpContext, AppDatabaseContext dbContext);
		public Task<UserModel> RegisterUserSessionAsync(HttpContext httpContext, AppDatabaseContext dbContext, string userName, string password);
	}
}
