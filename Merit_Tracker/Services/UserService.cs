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
        /// <summary>
        /// Gets the currently logged-in user based on session data from HttpContext.
        /// Returns null if no valid session or user found.
        /// </summary>
        public async Task<UserModel> GetCurrentUserAsync(HttpContext httpContext, AppDatabaseContext dbContext)
        {
            // Check if session exists and has keys
            if (httpContext.Session != null || httpContext.Session.Keys.Count() > 0)
            {
                // Try to get user ID and role bytes from session
                httpContext.Session.TryGetValue("UserID", out byte[] id);
                httpContext.Session.TryGetValue("Role", out byte[] role);

                // If either is missing, no valid user session
                if (id == null || role == null) return null;

                // If system architecture is little-endian, reverse byte order for correct int conversion
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(id);
                    Array.Reverse(role);
                }

                // Convert byte arrays to int for user ID
                int userID = BitConverter.ToInt32(id);

                // Query database for user with that ID
                return await dbContext.Users.FirstOrDefaultAsync(u => u.ID == userID);
            }

            // No valid session present
            return null;
        }

        /// <summary>
        /// Registers a user session by verifying username and password,
        /// then setting session variables if successful.
        /// Returns the user if authenticated, otherwise null.
        /// </summary>
        public async Task<UserModel> RegisterUserSessionAsync(HttpContext httpContext, AppDatabaseContext dbContext, string userName, string password)
        {
            // Hash the password using SHA256 (note: simple hashing, no salt here)
            SHA256 sha256 = SHA256.Create();
            string hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            sha256.Dispose();

            // Find user by username in database
            UserModel user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            // Check if user exists and passwords match
            if (user != null && user.Password == hashedPassword)
            {
                // Clear any existing session data
                httpContext.Session.Clear();

                // Store user ID and role in session for later retrieval
                httpContext.Session.SetInt32("UserID", user.ID);
                httpContext.Session.SetInt32("Role", (int)user.Role);

                // Return the authenticated user
                return user;
            }

            // Authentication failed
            return null;
        }
    }
}
