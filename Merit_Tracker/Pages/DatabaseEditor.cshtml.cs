using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Merit_Tracker.Pages
{
    // User specific databases
    public class DatabaseEditor : PageModel
    {
        public DatabaseModel CurrentDatabase { get; set; }
        public UserModel CurrentUser { get; private set; }
        public List<MeritModel> MeritRecords { get; set; } // Merit Records to be viewed

        // Modal Forms
        [BindProperty]
        public string MeritStudentName { get; set; }
		[BindProperty]
		public MeritValue MeritValue { get; set; }
		[BindProperty]
        public int MeritHousePoints { get; set; }

		public readonly AppDatabaseContext dbContext;

        public DatabaseEditor(AppDatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult OnGet(int databaseID)
        {
            var user = GetCurrentUser();
            if (user == null) return RedirectToPage("/Login"); // Return to login if current user is null or invalid

            CurrentUser = user;

            // Get the database matching the ID and user
            CurrentDatabase = GetDatabase(databaseID, CurrentUser);
           
            if (CurrentDatabase == null) return RedirectToPage("/Dashboard"); // If database doesn't belong to user, redirect back to dashboard
            // Get all merit records from the database
            MeritRecords = dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToList();

            return Page();
        }

		public string GetIssuerFullName(int userID)
		{
			var issuingUser = dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();
			return $"{issuingUser.FirstName} {issuingUser.LastName}";
		}

        public IActionResult OnPostAddMerit(int databaseID)
        {
			var user = GetCurrentUser();
			if (user == null) return RedirectToPage("/Login"); // Return to login if current user is null or invalid
			CurrentUser = user;


			// Get the database matching the ID and user
			CurrentDatabase = GetDatabase(databaseID, CurrentUser);

			if (CurrentDatabase == null) return RedirectToPage("/Dashboard"); // If database doesn't belong to user, redirect back to dashboard
			// Get all merit records from the database
			MeritRecords = dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToList();

			CurrentUser = user;

			// Add to database
			dbContext.Merits.Add(new MeritModel()
            {
                StudentName = MeritStudentName,
                HousePoints = MeritHousePoints,
                Value = MeritValue,
                DateOfIssue = DateTime.Now,
                IssuerID = CurrentUser.ID,
                DatabaseID = CurrentDatabase.DatabaseID,
                IssuerName = GetIssuerFullName(CurrentUser.ID),
            });

            dbContext.SaveChanges();
			MeritRecords = dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToList();

            return Partial("_DatabaseList", MeritRecords);
        }


        // Gets the current user from the session
        UserModel GetCurrentUser()
        {
			// Get current user
			HttpContext.Session.TryGetValue("UserID", out byte[] id);
			HttpContext.Session.TryGetValue("Role", out byte[] role);

            if (id == null || role == null) return null;

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(id);
				Array.Reverse(role);
			}

			int userID = BitConverter.ToInt32(id); //Get User ID from id bytes
			return dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();
		}


        DatabaseModel GetDatabase(int databaseID, UserModel user)
        {
            // Gets the matching database model from id and user
			var database = dbContext.Databases.Where(d => d.DatabaseID == databaseID)
			   .Where(d => d.UserID == user.ID).FirstOrDefault();
            return database;
		}


	}
}
