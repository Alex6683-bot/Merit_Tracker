using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit_Tracker.Models
{
    public enum MeritValue
    {
        Respect = 1,
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
        public MeritValue Value { get; set; }

        [Column("HousePoints")]
        public int HousePoints { get; set;}

        [Column("IssuerID")]
        public int IssuerID { get; set; }

		[Column("IssuerName")]
        public string IssuerName { get; set; }


		[Column("DatabaseID")]
        public int DatabaseID { get; set; }


    }


}
