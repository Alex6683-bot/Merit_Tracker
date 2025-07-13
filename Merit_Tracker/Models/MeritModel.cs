using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit_Tracker.Models
{
    public enum MeritValueType
    {
        Respect,
        Resilience,
        Endeavour,
        Compassion
    }

    [Table("Merits")]
    public class MeritModel
    {
        [Key]
        public int Id { get; set; }

        [Column("StudentName")]
        public string StudentName { get; set; }

        [Column("DateOfIssue")]
        public DateTime DateOfIssue { get; set; }

        [Column("Value")]
        public MeritValueType Value { get; set; }

        [Column("HousePoints")]
        public int HousePoints { get; set;}

        [Column("IssuerID")]
        public int IssuerID { get; set; }


        [Column("DatabaseID")]
        public int DatabaseID { get; set; }


    }
}
