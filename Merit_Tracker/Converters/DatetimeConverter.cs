using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Merit_Tracker.Converters
{
    // Converter to convert between utc and local time
    public class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeUtcConverter() : base(
            d => d.ToUniversalTime(),
            d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
        )
        {

        }
    }
}
