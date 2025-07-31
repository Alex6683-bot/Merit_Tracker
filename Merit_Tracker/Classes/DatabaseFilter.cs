using Merit_Tracker.Models;

namespace Merit_Tracker.Classes
{
	// This class is used to represent a filter for the database editor
	public class DatabaseFilter
	{
		public string? StudentNameFilter { get; set; } = "";
		public string? IssuerNameFilter { get; set; } = "";
		public int? MeritValueFilter { get; set; }
		public DateTime? MeritStartDateFilter { get; set; }
		public DateTime? MeritEndDateFilter { get; set; }
		public string? MeritYearLevelFilter { get; set; }

		public bool HasFilters()
		{
			return !string.IsNullOrWhiteSpace(StudentNameFilter)
				|| !string.IsNullOrWhiteSpace(IssuerNameFilter)
				|| MeritValueFilter.HasValue
				|| MeritStartDateFilter.HasValue
				|| MeritEndDateFilter.HasValue
				|| !string.IsNullOrWhiteSpace(MeritYearLevelFilter);
		}
	}
}
