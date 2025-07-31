using Merit_Tracker.Classes;
using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Merit_Tracker.Migrations;

namespace Merit_Tracker.Pages
{
    // PageModel class for editing and interacting with a specific user's merit database
    public class DatabaseEditor : PageModel
    {
        // The current database the user is viewing/editing
        public DatabaseModel CurrentDatabase { get; set; }

        // The currently authenticated user
        public UserModel CurrentUser { get; private set; }

        // The list of merit records displayed/edited on the page
        public List<MeritModel> MeritRecords { get; set; }

        // --- Modal Form Properties ---
        [BindProperty]
        [Required]
        public string MeritStudentName { get; set; }

        [BindProperty]
        [Required]
        public int MeritID { get; set; } // The ID of the merit being edited

        [BindProperty]
        [Required]
        public MeritValue MeritValue { get; set; } // The merit's value (enum)

        [BindProperty]
        [Required]
        public int MeritHousePoints { get; set; } // The number of house points assigned

        [BindProperty]
        [Required]
        public string MeritYearLevel { get; set; } // The year level assigned to merit in string


		// Year Levels list for combo box. Using strings seemed ideal and simpler for me for combo boxes.
		public List<SelectListItem> YearLevels = new List<SelectListItem>()
		{
			new SelectListItem() { Text = "Year 7" },
			new SelectListItem() { Text = "Year 8" },
			new SelectListItem() { Text = "Year 9" },
			new SelectListItem() { Text = "Year 10" },
			new SelectListItem() { Text = "Year 11" },
			new SelectListItem() { Text = "Year 12" },

		};

		// --- Services and Database Context ---
		public readonly AppDatabaseContext dbContext;
        private readonly IUserService userService;
        public readonly IUserMeritDatabaseService userMeritDatabaseService;

        // Constructor to inject dependencies
        public DatabaseEditor(AppDatabaseContext dbContext, IUserService userService, IUserMeritDatabaseService userDatabaseService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
            this.userMeritDatabaseService = userDatabaseService;
        }

        // Loads the database editor view
        public async Task<IActionResult> OnGetAsync(int databaseID, string? studentName, string? issuerName, int? value, DateTime? startDate, DateTime? endDate, string yearLevel)
        {
			var result = await InitializeEditorAsync(databaseID, new DatabaseFilter()
			{
				StudentNameFilter = studentName,
				IssuerNameFilter = issuerName,
				MeritValueFilter = value,
				MeritStartDateFilter = startDate,
				MeritEndDateFilter = endDate,
				MeritYearLevelFilter = yearLevel
			});
			if (result != null) return result;

            return Page();
        }

        // Gets the full name of the user issuing a merit
        public async Task<string> GetIssuerFullNameAsync(int userID)
        {
            var issuingUser = await dbContext.Users.Where(u => u.ID == userID).FirstOrDefaultAsync();
            return $"{issuingUser.FirstName} {issuingUser.LastName}";
        }

        // POST: Adds a new merit to the database
        public async Task<IActionResult> OnPostAddMeritAsync(int databaseID, string? studentName, string? issuerName, int? value, DateTime? startDate, DateTime? endDate, string yearLevel)
        {
			var result = await InitializeEditorAsync(databaseID, new DatabaseFilter()
			{
				StudentNameFilter = studentName,
				IssuerNameFilter = issuerName,
				MeritValueFilter = value,
				MeritStartDateFilter = startDate,
				MeritEndDateFilter = endDate,
				MeritYearLevelFilter = yearLevel
			});
			if (result != null) return result;

            // Add merit to database
            MeritRecords = await userMeritDatabaseService.AddRecordToDatabaseAsync(
                new MeritAddRequest()
                {
                    MeritStudentName = this.MeritStudentName,
                    MeritHousePoints = this.MeritHousePoints,
                    MeritValue = this.MeritValue,
                    MeritIssuerID = CurrentUser.ID,
                    MeritYearLevel = this.MeritYearLevel,
                    MeritIssuerName = await GetIssuerFullNameAsync(CurrentUser.ID)

                },
                CurrentDatabase,
                dbContext); ;

            // Filter to only user's records if not admin
            if (CurrentUser.Role != UserRole.Admin)
                MeritRecords = MeritRecords.Where(m => m.IssuerID == CurrentUser.ID).ToList();

			MeritRecords = await FilterMeritsAsync(userMeritDatabaseService.DatabaseFilter);

			return Partial("_DatabaseList", this);
        }

        // POST: Edits an existing merit in the database
        public async Task<IActionResult> OnPostEditMeritAsync(int databaseID, string? studentName, string? issuerName, int? value, DateTime? startDate, DateTime? endDate, string yearLevel)
        {
			var result = await InitializeEditorAsync(databaseID, new DatabaseFilter()
			{
				StudentNameFilter = studentName,
				IssuerNameFilter = issuerName,
				MeritValueFilter = value,
				MeritStartDateFilter = startDate,
				MeritEndDateFilter = endDate,
				MeritYearLevelFilter = yearLevel
			});
			if (result != null) return result;

            if (CurrentUser.Role == UserRole.Admin)
			{
				var editResult = await userMeritDatabaseService.EditRecordInDatabaseAsync(
                    new MeritEditRequest()
                    {
                        MeritStudentName = this.MeritStudentName,
                        MeritHousePoints = this.MeritHousePoints,
                        MeritYearLevel = this.MeritYearLevel,
                        MeritValue = this.MeritValue,
                    },
                    MeritID,
                    CurrentDatabase,
                    dbContext);

                if (editResult == null) return StatusCode(400); // Edit failed
                MeritRecords = editResult;
                MeritRecords = await FilterMeritsAsync(userMeritDatabaseService.DatabaseFilter);
            }

            return Partial("_DatabaseList", this);
        }

        // POST: Deletes a merit from the database
        public async Task<IActionResult> OnPostDeleteMeritAsync(int databaseID, int meritId, string? studentName, string? issuerName, int? value, DateTime? startDate, DateTime? endDate, string yearLevel)
        {
			var result = await InitializeEditorAsync(databaseID, new DatabaseFilter()
			{
				StudentNameFilter = studentName,
				IssuerNameFilter = issuerName,
				MeritValueFilter = value,
				MeritStartDateFilter = startDate,
				MeritEndDateFilter = endDate,
				MeritYearLevelFilter = yearLevel
			});
			if (result != null) return result;

            if (CurrentUser.Role == UserRole.Admin)
            {
                var deleteResult = await userMeritDatabaseService.DeleteRecordFromDatabaseAsync(
                    meritId, dbContext, CurrentDatabase);

                if (deleteResult == null) return StatusCode(400); // Delete failed

                MeritRecords = deleteResult;
				MeritRecords = await FilterMeritsAsync(userMeritDatabaseService.DatabaseFilter);
			}

            return Partial("_DatabaseList", this);
        }

        // POST: Filters the merit database based on multiple optional fields. Parameters are retrieved from route values.
        public async Task<IActionResult> OnPostFilterMeritAsync(int databaseID, string? studentName, string? issuerName, int? value, DateTime? startDate, DateTime? endDate, string yearLevel)
        {
            var result = await InitializeEditorAsync(databaseID, new DatabaseFilter() // Create filter from parameters and pass it 
			{
				StudentNameFilter = studentName,
				IssuerNameFilter = issuerName,
				MeritValueFilter = value,
				MeritStartDateFilter = startDate,
				MeritEndDateFilter = endDate,
				MeritYearLevelFilter = yearLevel
			});
            if (result != null) return result;


            //// Set filtered state to true;
            //userMeritDatabaseService.IsFiltered = true;
            //userMeritDatabaseService.DatabaseFilter = databaseFilter;

            //var filterResult = await userMeritDatabaseService.FilterMeritDatabaseAsync(databaseFilter, dbContext, CurrentDatabase);
            //if (filterResult == null) return Partial("_DatabaseList", this);

            //// Only show user's records unless admin
            //if (CurrentUser.Role == UserRole.Admin)
            //    MeritRecords = filterResult;
            //else
            //    MeritRecords = filterResult.Where(m => m.IssuerID == CurrentUser.ID).ToList();

            return Partial("_DatabaseList", this);
        }

        // POST: Removes any applied filters from the merit database
        public async Task<IActionResult> OnPostRemoveFilterAsync(int databaseID, string? studentName, string? issuerName, int? value, DateTime? startDate, DateTime? endDate, string yearLevel)
        {
            var result = await InitializeEditorAsync(databaseID, null); // Passing null as database filter to remove filter
            if (result != null) return result;

			return Partial("_DatabaseList", this);
        }

        // Initializes core properties and verifies user access to the requested database
        public async Task<IActionResult> InitializeEditorAsync(int databaseID, DatabaseFilter databaseFilter)
        {
            var user = await userService.GetCurrentUserAsync(HttpContext, dbContext);
            if (user == null) return RedirectToPage("/Login");
            CurrentUser = user;

			// Load the user-specific or admin-accessible database
			CurrentDatabase = await userMeritDatabaseService.GetDatabaseAsync(databaseID, user, dbContext);
            if (CurrentDatabase == null) return RedirectToPage("/Dashboard");


			// Get currently filtered merits
			var meritRecords = await FilterMeritsAsync(databaseFilter);

            MeritRecords = meritRecords;


            return null;
        }

        // Returns filtered merit records from given filter
		async Task<List<MeritModel>> FilterMeritsAsync(DatabaseFilter databaseFilter)
        {
            if (databaseFilter != null && databaseFilter.HasFilters())
            {
                // Set filtered state to true;
                userMeritDatabaseService.IsFiltered = true;
                userMeritDatabaseService.DatabaseFilter = databaseFilter;
            }
            else userMeritDatabaseService.IsFiltered = false;

            var filteredMeritRecords = await userMeritDatabaseService.FilterMeritDatabaseAsync(databaseFilter, dbContext, CurrentDatabase);

			if (CurrentUser.Role != UserRole.Admin) // If it is not admin, only merits posted by that account can be see
				filteredMeritRecords = filteredMeritRecords
	            .Where(m => m.DatabaseID == CurrentDatabase.DatabaseID && m.IssuerID == CurrentUser.ID)
	            .ToList();

			return filteredMeritRecords;

		}
	}
}
