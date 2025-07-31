using Merit_Tracker.Classes;
using Merit_Tracker.Database;
using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Merit_Tracker.Interfaces
{
	// Interface used to implement servies that handle merit database
	public interface IUserMeritDatabaseService
	{
		public bool IsFiltered { get; set; }
		public DatabaseFilter DatabaseFilter { get; set; }


		public Task<List<MeritModel>> AddRecordToDatabaseAsync(
			MeritAddRequest meritAdd,
			DatabaseModel currentDatabaseModel,
			AppDatabaseContext dbContext
		);

		public Task<List<MeritModel>> EditRecordInDatabaseAsync(MeritEditRequest meritEdit, int meritID, DatabaseModel currentDatabaseModel, AppDatabaseContext dbContext);
		public Task<List<MeritModel>> DeleteRecordFromDatabaseAsync(int meritID, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel);
		public Task<DatabaseModel> GetDatabaseAsync(int databaseId, UserModel userModel, AppDatabaseContext dbContext);
		public Task<List<MeritModel>> FilterMeritDatabaseAsync(DatabaseFilter filter, AppDatabaseContext dbContext, DatabaseModel currentDatabaseModel);
	}
}
