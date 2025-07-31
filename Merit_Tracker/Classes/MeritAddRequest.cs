namespace Merit_Tracker.Classes
{
	// Class used to hold data related to adding merits to the database. This class is mainly created to make code less messier and reduce the number of parameters for edit call.
	// To reduce code repetition, it is inherited by MeritEditRequest as it have some fields in commmon
	public class MeritAddRequest : MeritEditRequest
	{
		public int MeritIssuerID { get; set; }
		public string MeritIssuerName { get; set; }
	}
}
