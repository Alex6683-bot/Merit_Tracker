using Merit_Tracker.Classes;
using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Merit_Tracker.Interfaces
{
	public interface IUserMeritDatabaseService
	{
		public bool IsFiltered { get; set; }
		public DatabaseFilter DatabaseFilter { get; set; }


		public Task<List<MeritModel>> AddRecordToDatabaseAsync(
			string meritName,
			int meritHousePoints,
			MeritValue meritValue,
			int meritIssuerID,
			string meritIssuerName,
			DatabaseModel currentDatabaseModel,
			AppDatabaseContext dbContext
		);

		public Task<List<MeritModel>> EditRecordInDatabaseAsync(string meritStudentName, MeritValue meritValue, int meritHousePoints, int meritID, DatabaseModel currentDatabaseModel, AppDatabaseContext dbContext);
		public Task<List<MeritModel>> DeleteRecordFromDatabaseAsync(int meritID, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel);
		public Task<DatabaseModel> GetDatabaseAsync(int databaseId, UserModel userModel, AppDatabaseContext dbContext);
		public Task<List<MeritModel>> FilterMeritDatabaseAsync(DatabaseFilter filter, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel);
	}
}
