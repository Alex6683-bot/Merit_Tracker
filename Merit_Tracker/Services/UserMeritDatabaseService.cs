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
		public bool IsFiltered { get; set; } = false;
		public DatabaseFilter DatabaseFilter { get; set; } = new DatabaseFilter();


		public async Task<List<MeritModel>> AddRecordToDatabaseAsync(
			string meritStudentName,
			int meritHousePoints,
			MeritValue meritValue,
			int meritIssuerID,
			string meritIssuerName,
			DatabaseModel currentDatabaseModel,
			AppDatabaseContext dbContext
		)
		{
			dbContext.Merits.Add(new MeritModel()
			{
				StudentName = meritStudentName.Trim(), // Trimming leading and following spaces for names
				HousePoints = meritHousePoints,
				Value = meritValue,
				DateOfIssue = DateTime.Now,
				IssuerID = meritIssuerID,
				DatabaseID = currentDatabaseModel.DatabaseID,
				IssuerName = meritIssuerName.Trim()
			});
			await dbContext.SaveChangesAsync();
			return await dbContext.Merits.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID).ToListAsync();
		}

		public async Task<List<MeritModel>> EditRecordInDatabaseAsync(string meritStudentName, MeritValue meritValue, int meritHousePoints,
			int meritID, DatabaseModel currentDatabaseModel, AppDatabaseContext dbContext)
		{
			// Get merit from database
			MeritModel merit = await dbContext.Merits.Where(m => m.Id == meritID).FirstOrDefaultAsync();
			if (merit == null) return null;

			// Edit properties of record to given values
			merit.StudentName = meritStudentName.Trim(); // Trimming leading and following spaces
			merit.Value = meritValue;
			merit.HousePoints = meritHousePoints;

			await dbContext.SaveChangesAsync();
			return await dbContext.Merits.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID).ToListAsync();
		}

		// Get the database matching the ID and user
		public async Task<DatabaseModel> GetDatabaseAsync(int databaseID, UserModel user, AppDatabaseContext dbContext)
		{
			DatabaseModel database = await dbContext.Databases.Where(d => d.DatabaseID == databaseID)
				.FirstOrDefaultAsync(d => d.UserID == user.ID);
			return database;
		}

		public async Task<List<MeritModel>> DeleteRecordFromDatabaseAsync(int meritID, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel)
		{
			MeritModel meritModel = await dbContext.Merits.FirstOrDefaultAsync(m => m.Id == meritID);
			if (meritModel == null) return null;	

			dbContext.Merits.Remove(meritModel);
			await dbContext.SaveChangesAsync();

			return await dbContext.Merits.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID).ToListAsync();

		}

		public async Task<List<MeritModel>> FilterMeritDatabaseAsync(DatabaseFilter filter, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel)
		{
			var query = dbContext.Merits.AsQueryable();

			query = query.Where(m => m.DatabaseID == currentDatabaseModel.DatabaseID);

			if (!string.IsNullOrWhiteSpace(filter.StudentNameFilter))
				query = query.Where(m => m.StudentName.Contains(filter.StudentNameFilter));

			if (!string.IsNullOrWhiteSpace(filter.IssuerNameFilter))
				query = query.Where(m => m.IssuerName.Contains(filter.IssuerNameFilter));

			if (filter.MeritValueFilter > 0)
				query = query.Where(m => m.Value == (MeritValue)filter.MeritValueFilter);

			if (filter.MeritStartDateFilter.HasValue)
				query = query.Where(m => m.DateOfIssue >= filter.MeritStartDateFilter.Value.ToUniversalTime());

			if (filter.MeritEndDateFilter.HasValue)
				query = query.Where(m => m.DateOfIssue <= filter.MeritEndDateFilter.Value.ToUniversalTime());

			return await query.ToListAsync();

		}
	}

}
