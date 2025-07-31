using Merit_Tracker.Classes;
using Merit_Tracker.Database;
using Merit_Tracker.Interfaces;
using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Runtime.CompilerServices;
using System.Text;

namespace Merit_Tracker.Services
{
    public class UserMeritDatabaseService : IUserMeritDatabaseService
    {
        // Indicates whether the current data is filtered
        public bool IsFiltered { get; set; } = false;

        // Holds current filter state for merit filtering
        public DatabaseFilter DatabaseFilter { get; set; } = new DatabaseFilter();

        /// <summary>
        /// Adds a new merit record to the database for a given student and returns the updated list.
        /// </summary>
        public async Task<List<MeritModel>?> AddRecordToDatabaseAsync(
            MeritAddRequest meritAdd,
            DatabaseModel currentDatabaseModel,
            AppDatabaseContext dbContext
        )
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(meritAdd.MeritStudentName) ||
                string.IsNullOrWhiteSpace(meritAdd.MeritIssuerName) ||
                string.IsNullOrWhiteSpace(meritAdd.MeritYearLevel) ||
                currentDatabaseModel == null ||
                dbContext == null ||
                meritAdd.MeritHousePoints < 0 ||
                meritAdd.MeritIssuerID <= 0)
            {
                return null;
            }

            // Create new merit record and add to DB
            dbContext.Merits.Add(new MeritModel()
            {
                StudentName = meritAdd.MeritStudentName.Trim(),
                HousePoints = meritAdd.MeritHousePoints,
                Value = meritAdd.MeritValue,
                DateOfIssue = DateTime.UtcNow,
                IssuerID = meritAdd.MeritIssuerID,
                DatabaseID = currentDatabaseModel.DatabaseID,
                IssuerName = meritAdd.MeritIssuerName.Trim(),
                Yearlevel = meritAdd.MeritYearLevel.Trim()
            }); ;

            // Save to DB
            await dbContext.SaveChangesAsync();

            // Return updated merit list for this database
            return await dbContext.Merits
                .Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID)
                .ToListAsync();
        }

        /// <summary>
        /// Edits a merit record in the database with new values and returns the updated merit list.
        /// </summary>
        public async Task<List<MeritModel>> EditRecordInDatabaseAsync(MeritEditRequest meritEdit, int meritID, DatabaseModel currentDatabaseModel, AppDatabaseContext dbContext)
        {
            // Fetch merit by ID
            MeritModel merit = await dbContext.Merits.Where(m => m.Id == meritID).FirstOrDefaultAsync();
            if (merit == null) return null;

            // Validate inputs before editing
            if (!string.IsNullOrWhiteSpace(meritEdit.MeritStudentName) && !string.IsNullOrWhiteSpace(meritEdit.MeritYearLevel) && meritEdit.MeritHousePoints != null && meritEdit.MeritValue != null && meritEdit.MeritHousePoints >= 0)
            {
                // Update properties
                merit.StudentName = meritEdit.MeritStudentName.Trim();
                merit.Value = meritEdit.MeritValue;
                merit.HousePoints = meritEdit.MeritHousePoints;
                merit.Yearlevel = meritEdit.MeritYearLevel.Trim();

                await dbContext.SaveChangesAsync();

                // Return updated list
                return await dbContext.Merits.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID).ToListAsync();
            }
            return null;
        }

        /// <summary>
        /// Fetches a database by ID that belongs to the given user.
        /// </summary>
        public async Task<DatabaseModel> GetDatabaseAsync(int databaseID, UserModel user, AppDatabaseContext dbContext)
        {
            // Find database owned by this user
            DatabaseModel database = await dbContext.Databases.Where(d => d.DatabaseID == databaseID)
                .FirstOrDefaultAsync(d => d.UserID == user.ID);
            return database;
        }

        /// <summary>
        /// Deletes a merit record from the database and returns the updated merit list.
        /// </summary>
        public async Task<List<MeritModel>> DeleteRecordFromDatabaseAsync(int meritID, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel)
        {
            // Find merit record
            MeritModel meritModel = await dbContext.Merits.FirstOrDefaultAsync(m => m.Id == meritID);
            if (meritModel == null) return null;

            // Remove and save
            dbContext.Merits.Remove(meritModel);
            await dbContext.SaveChangesAsync();

            // Return updated list
            return await dbContext.Merits.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID).ToListAsync();
        }

        /// <summary>
        /// Applies filtering to the merit database and returns the filtered list of merits.
        /// </summary>
        public async Task<List<MeritModel>> FilterMeritDatabaseAsync(DatabaseFilter filter, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel)
        {
            var baseQuery = dbContext.Merits.AsQueryable();
            baseQuery = baseQuery.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID);

			if (filter != null)
            {
                // Start with base query from current database
                var query = baseQuery;

                // Apply filters conditionally
                if (!string.IsNullOrWhiteSpace(filter.StudentNameFilter))
                    query = query.Where(m => m.StudentName.Contains(filter.StudentNameFilter));

                if (!string.IsNullOrWhiteSpace(filter.IssuerNameFilter))
                    query = query.Where(m => m.IssuerName.Contains(filter.IssuerNameFilter));

                if (!string.IsNullOrWhiteSpace(filter.MeritYearLevelFilter))
                    query = query.Where(m => m.Yearlevel == filter.MeritYearLevelFilter);

                if (filter.MeritValueFilter > 0)
                    query = query.Where(m => m.Value == (MeritValue)filter.MeritValueFilter);

                if (filter.MeritStartDateFilter.HasValue)
                    query = query.Where(m => m.DateOfIssue >= filter.MeritStartDateFilter.Value.Date.ToUniversalTime());

                if (filter.MeritEndDateFilter.HasValue)
                {
                    var endDate = filter.MeritEndDateFilter.Value.Date.AddDays(1).ToUniversalTime();
                    query = query.Where(m => m.DateOfIssue < endDate);
                }

                // Return filtered results
                return await query.ToListAsync();
            }
            // return base query if other filters are null
            return await baseQuery.ToListAsync();
        }
    }
}
