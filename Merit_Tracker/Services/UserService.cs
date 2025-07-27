using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Merit_Tracker.Services
{
	public class UserService : IUserService
	{		 
		public async Task<UserModel> GetCurrentUserAsync(HttpContext httpContext, AppDatabaseContext dbContext)
		{
			// Get current user
			if (httpContext.Session != null || httpContext.Session.Keys.Count() > 0)
			{
				httpContext.Session.TryGetValue("UserID", out byte[] id);
				httpContext.Session.TryGetValue("Role", out byte[] role);

				if (id == null || role == null) return null;

				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(id);
					Array.Reverse(role);
				}
				int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
				return await dbContext.Users.FirstOrDefaultAsync(u => u.ID == userID);
			}
			return null;
		}

		public async Task<UserModel> RegisterUserSessionAsync(HttpContext httpContext, AppDatabaseContext dbContext, string userName, string password)
		{
			// SHA 256 is the built in class used to encrypt the password, although it is not the best way, I decided to use it for the time being
			SHA256 sha256 = SHA256.Create();
			string hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
			sha256.Dispose();

			UserModel user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

			if (user != null && user.Password == hashedPassword)
			{
				httpContext.Session.Clear();
				httpContext.Session.SetInt32("UserID", user.ID);
				httpContext.Session.SetInt32("Role", (int)user.Role);

				return user;


			}
			return null;
		}




    }
}
