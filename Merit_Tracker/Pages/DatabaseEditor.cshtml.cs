using Merit_Tracker.Database;
using Merit_Tracker.Helpers;
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
        public int MeritID { get; set; } // Currently selected merit's id
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
			var result = InitializeEditor(databaseID);

			if (result != null) return result; // Return the page result if it doesn't return null

			return Page();
        }

		public string GetIssuerFullName(int userID)
		{
			var issuingUser = dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();
			return $"{issuingUser.FirstName} {issuingUser.LastName}";
		}


        // Handler for adding merit
        public IActionResult OnPostAddMerit(int databaseID)
        {
			var result = InitializeEditor(databaseID);

			if (result != null) return result; // Return the page result if it doesn't return null

			// Add to database
			dbContext.Merits.Add(new MeritModel()
            {
                StudentName = MeritStudentName.Trim(), // Trimming leading and following spaces for names
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

        // Handler for editing merit
        public IActionResult OnPostEditMerit(int databaseID)
        {
            var result = InitializeEditor(databaseID);

            if (result != null) return result; // Return the page result if it doesn't return null

            // Get merit from database
            MeritModel merit = dbContext.Merits.Where(m => m.Id == MeritID).FirstOrDefault();
            if (merit == null) return StatusCode(500);

            // Edit properties of record to form inputs
            merit.StudentName = MeritStudentName.Trim(); // Trimming leading and following spaces
            merit.Value = MeritValue;
            merit.HousePoints = MeritHousePoints;

			dbContext.SaveChanges();
			MeritRecords = dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToList();

			return Partial("_DatabaseList", MeritRecords);
		}



        // Method to initialize all properties in this model
        public IActionResult InitializeEditor(int databaseID)
        {
			var user = HelperMethods.GetCurrentUser(HttpContext, dbContext);
			if (user == null) return RedirectToPage("/Login"); // Return to login if current user is null or invalid
			CurrentUser = user;

			// Get the database matching the ID and user
			CurrentDatabase = HelperMethods.GetDatabase(databaseID, CurrentUser, dbContext);

			if (CurrentDatabase == null) return RedirectToPage("/Dashboard"); // If database doesn't belong to user, redirect back to dashboard
			// Get all merit records from the database
			MeritRecords = dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToList();

            return null;
		}

	}
}
