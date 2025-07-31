using Merit_Tracker.Models;

namespace Merit_Tracker.Classes
{
	// Class used to hold data regarding the edits made to a merit record. This class is mainly created to make code less messier and reduce the number of parameters for edit call.
	public class MeritEditRequest
	{
		public string MeritStudentName { get; set; }
		public MeritValue MeritValue { get; set; }
		public string MeritYearLevel { get; set; }
		public int MeritHousePoints { get; set; }

	}
}
