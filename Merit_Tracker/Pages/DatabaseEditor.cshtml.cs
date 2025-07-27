using Merit_Tracker.Classes;
using Merit_Tracker.Database;
using Merit_Tracker.Helpers;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

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
        private readonly IUserService userService;
        public readonly IUserMeritDatabaseService userDatabaseService;

        public DatabaseEditor(AppDatabaseContext dbContext, IUserService userService, IUserMeritDatabaseService userDatabaseService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
            this.userDatabaseService = userDatabaseService;
        }

        public async Task<IActionResult> OnGetAsync(int databaseID)
        {
			var result = await InitializeEditorAsync(databaseID);

			if (result != null) return result; // Return the page result if it doesn't return null

			return Page();
        }

		public async Task<string> GetIssuerFullNameAsync(int userID)
		{
			var issuingUser = await dbContext.Users.Where(u => u.ID == userID).FirstOrDefaultAsync();
			return $"{issuingUser.FirstName} {issuingUser.LastName}";
		}


        // Handler end point for adding merit
        public async Task<IActionResult> OnPostAddMeritAsync(int databaseID)
        {
			var result = await InitializeEditorAsync(databaseID);

			if (result != null) return result; // Return the page result if it doesn't return null

            // Add to database and get the updated database list
            MeritRecords = await userDatabaseService.AddRecordToDatabaseAsync(
                MeritStudentName,
                MeritHousePoints,
                MeritValue,
                CurrentUser.ID,
                await GetIssuerFullNameAsync(CurrentUser.ID),
                CurrentDatabase,
                dbContext);

            return Partial("_DatabaseList", this);
        }

        // Handler end point for editing merit
        public async Task<IActionResult> OnPostEditMeritAsync(int databaseID)
        {
            var result = await InitializeEditorAsync(databaseID);

            if (result != null) return result; // Return the page result if it doesn't return null

            // Edit selected merit record through its id. It returns either null or the updated merit list
            var editResult = await userDatabaseService.EditRecordInDatabaseAsync(MeritStudentName,
                MeritValue,
                MeritHousePoints,
                MeritID,
                CurrentDatabase,
                dbContext);

            if (editResult == null) return StatusCode(400); // Failed edit if it returns null
            MeritRecords = editResult;

			return Partial("_DatabaseList", this);

		}

		// Handler end point for deleting merit models
		public async Task<IActionResult> OnPostDeleteMeritAsync(int databaseID, int meritId)
        {
			var result = await InitializeEditorAsync(databaseID);
			if (result != null) return result; // Return the page result if it doesn't return null

			var deleteResult = await userDatabaseService.DeleteRecordFromDatabaseAsync(meritId, dbContext, CurrentDatabase);

			if (deleteResult == null) return StatusCode(400); // Failed edit if it returns null
			MeritRecords = deleteResult;

			return Partial("_DatabaseList", this);

		}

		// Handler end point for filtering merit records
		public async Task<IActionResult> OnPostFilterMeritAsync(int databaseID, int meritID)
        {
			var result = await InitializeEditorAsync(databaseID);
			if (result != null) return result; // Return the page result if it doesn't return null

            // Get raw json
            DatabaseFilter? databaseFilter;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string json = await reader.ReadToEndAsync();
                databaseFilter =  JsonSerializer.Deserialize<DatabaseFilter>(json);
            }

            if (databaseFilter == null) return Partial("_DatabaseList", this);
            userDatabaseService.IsFiltered = true;
			userDatabaseService.DatabaseFilter = databaseFilter;

            var filterResult = await userDatabaseService.FilterMeritDatabaseAsync(databaseFilter, dbContext, CurrentDatabase);
            if (filterResult == null) return Partial("_DatabaseList", this);

            MeritRecords = filterResult;

			return Partial("_DatabaseList", this);

		}

        // Handlet end point for removing filter
        public async Task<IActionResult> OnPostRemoveFilterAsync(int databaseID, int meritID)
        {
			var result = await InitializeEditorAsync(databaseID);
			if (result != null) return result; // Return the page result if it doesn't return null

            userDatabaseService.IsFiltered = false;
            // Set filter values to an empty object
            userDatabaseService.DatabaseFilter = new DatabaseFilter();

			return Partial("_DatabaseList", this);
		}


		// Method to initialize all properties in this model
		public async Task<IActionResult> InitializeEditorAsync(int databaseID)
        {
			var user = await userService.GetCurrentUserAsync(HttpContext, dbContext);
			if (user == null) return RedirectToPage("/Login"); // Return to login if current user is null or invalid
			CurrentUser = user;

			// Get the database matching the ID and user
			CurrentDatabase = await userDatabaseService.GetDatabaseAsync(databaseID, user, dbContext);

			if (CurrentDatabase == null) return RedirectToPage("/Dashboard"); // If database doesn't belong to user, redirect back to dashboard

			// Get all merit records from the database
			MeritRecords = await dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToListAsync();

            return null;
		}


	}
}
