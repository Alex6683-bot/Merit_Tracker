using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Merit_Tracker.Interfaces
{
	public interface IUserDatabaseService
	{
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

	}
}
