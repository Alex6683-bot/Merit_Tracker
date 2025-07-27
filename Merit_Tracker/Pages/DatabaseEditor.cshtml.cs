using Merit_Tracker.Classes;
using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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

        // --- Services and Database Context ---
        public readonly AppDatabaseContext dbContext;
        private readonly IUserService userService;
        public readonly IUserMeritDatabaseService userDatabaseService;

        // Constructor to inject dependencies
        public DatabaseEditor(AppDatabaseContext dbContext, IUserService userService, IUserMeritDatabaseService userDatabaseService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
            this.userDatabaseService = userDatabaseService;
        }

        // Loads the database editor view
        public async Task<IActionResult> OnGetAsync(int databaseID)
        {
            var result = await InitializeEditorAsync(databaseID);
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
        public async Task<IActionResult> OnPostAddMeritAsync(int databaseID)
        {
            var result = await InitializeEditorAsync(databaseID);
            if (result != null) return result;

            // Add merit to database
            MeritRecords = await userDatabaseService.AddRecordToDatabaseAsync(
                MeritStudentName,
                MeritHousePoints,
                MeritValue,
                CurrentUser.ID,
                await GetIssuerFullNameAsync(CurrentUser.ID),
                CurrentDatabase,
                dbContext);

            // Filter to only user's records if not admin
            if (CurrentUser.Role != UserRole.Admin)
                MeritRecords = MeritRecords.Where(m => m.IssuerID == CurrentUser.ID).ToList();

            return Partial("_DatabaseList", this);
        }

        // POST: Edits an existing merit in the database
        public async Task<IActionResult> OnPostEditMeritAsync(int databaseID)
        {
            var result = await InitializeEditorAsync(databaseID);
            if (result != null) return result;

            if (CurrentUser.Role == UserRole.Admin)
            {
                var editResult = await userDatabaseService.EditRecordInDatabaseAsync(
                    MeritStudentName,
                    MeritValue,
                    MeritHousePoints,
                    MeritID,
                    CurrentDatabase,
                    dbContext);

                if (editResult == null) return StatusCode(400); // Edit failed
                MeritRecords = editResult;
            }

            return Partial("_DatabaseList", this);
        }

        // POST: Deletes a merit from the database
        public async Task<IActionResult> OnPostDeleteMeritAsync(int databaseID, int meritId)
        {
            var result = await InitializeEditorAsync(databaseID);
            if (result != null) return result;

            if (CurrentUser.Role == UserRole.Admin)
            {
                var deleteResult = await userDatabaseService.DeleteRecordFromDatabaseAsync(
                    meritId, dbContext, CurrentDatabase);

                if (deleteResult == null) return StatusCode(400); // Delete failed
                MeritRecords = deleteResult;
            }

            return Partial("_DatabaseList", this);
        }

        // POST: Filters the merit database based on multiple optional fields
        public async Task<IActionResult> OnPostFilterMeritAsync(int databaseID, int meritID, string? StudentName, string? IssuerName, int? Value, DateTime? StartDate, DateTime? EndDate)
        {
            var result = await InitializeEditorAsync(databaseID);
            if (result != null) return result;

            // Create a filter object from provided parameters
            DatabaseFilter? databaseFilter = new DatabaseFilter()
            {
                StudentNameFilter = StudentName,
                IssuerNameFilter = IssuerName,
                MeritValueFilter = Value,
                MeritStartDateFilter = StartDate,
                MeritEndDateFilter = EndDate
            };

            if (databaseFilter == null) return Partial("_DatabaseList", this);

            userDatabaseService.IsFiltered = true;
            userDatabaseService.DatabaseFilter = databaseFilter;

            var filterResult = await userDatabaseService.FilterMeritDatabaseAsync(databaseFilter, dbContext, CurrentDatabase);
            if (filterResult == null) return Partial("_DatabaseList", this);

            // Only show user's records unless admin
            if (CurrentUser.Role == UserRole.Admin)
                MeritRecords = filterResult;
            else
                MeritRecords = filterResult.Where(m => m.IssuerID == CurrentUser.ID).ToList();

            return Partial("_DatabaseList", this);
        }

        // POST: Removes any applied filters from the merit database
        public async Task<IActionResult> OnPostRemoveFilterAsync(int databaseID, int meritID)
        {
            var result = await InitializeEditorAsync(databaseID);
            if (result != null) return result;

            userDatabaseService.IsFiltered = false;
            userDatabaseService.DatabaseFilter = new DatabaseFilter();

            return Partial("_DatabaseList", this);
        }

        // Initializes core properties and verifies user access to the requested database
        public async Task<IActionResult> InitializeEditorAsync(int databaseID)
        {
            var user = await userService.GetCurrentUserAsync(HttpContext, dbContext);
            if (user == null) return RedirectToPage("/Login");

            CurrentUser = user;

            // Load the user-specific or admin-accessible database
            CurrentDatabase = await userDatabaseService.GetDatabaseAsync(databaseID, user, dbContext);
            if (CurrentDatabase == null) return RedirectToPage("/Dashboard");

            // Load all merit records depending on user role
            if (CurrentUser.Role == UserRole.Admin)
                MeritRecords = await dbContext.Merits.Where(m => m.DatabaseID == CurrentDatabase.DatabaseID).ToListAsync();
            else
                MeritRecords = await dbContext.Merits
                    .Where(m => m.DatabaseID == CurrentDatabase.DatabaseID && m.IssuerID == CurrentUser.ID)
                    .ToListAsync();

            return null;
        }
    }
}
