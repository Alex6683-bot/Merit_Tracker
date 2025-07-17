using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Merit_Tracker.Helpers
{
	public static class HelperMethods
	{
		// Gets the user from session
		public static UserModel GetCurrentUser(HttpContext httpContext, AppDatabaseContext dbContext)
		{
			// Get current user
			httpContext.Session.TryGetValue("UserID", out byte[] id);
			httpContext.Session.TryGetValue("Role", out byte[] role);

			if (id == null || role == null) return null;

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(id);
				Array.Reverse(role);
			}
			int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
			return dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();
		}

		public static DatabaseModel GetDatabase(int databaseID, UserModel user, AppDatabaseContext dbContext)
		{
			// Gets the matching database model from id and user
			var database = dbContext.Databases.Where(d => d.DatabaseID == databaseID)
			   .Where(d => d.UserID == user.ID).FirstOrDefault();
			return database;
		}
	}
}
