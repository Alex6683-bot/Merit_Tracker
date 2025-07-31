using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit_Tracker.Models
{
    // Enum representing the possible values a student can earn merits for
    public enum MeritValue
    {
        Respect = 1,      // Merit for respect
        Resilience,       // Merit for resilience
        Endeavour,        // Merit for endeavour
        Compassion        // Merit for compassion
    }


    // Model used for representing a merit record stored in the "Merits" table
    [Table("Merits")]
    public class MeritModel
    {


        // Primary key for each merit record
        [Key]
        public int Id { get; set; }

        // Name of the student receiving the merit
        [Column("StudentName")]
        public string StudentName { get; set; }

        // Date and time the merit was issued
        [Column("DateOfIssue")]
        public DateTime DateOfIssue { get; set; }

        // Value of the merit (e.g. Respect, Resilience, etc.)
        [Column("Value")]
        public MeritValue Value { get; set; }

        // Number of house points associated with this merit
        [Column("HousePoints")]
        public int HousePoints { get; set; }

        // ID of the teacher or person who issued the merit
        [Column("IssuerID")]
        public int IssuerID { get; set; }

        // Name of the teacher or person who issued the merit
        [Column("IssuerName")]
        public string IssuerName { get; set; }

        // The ID of the database (used to associate merits with a specific school or environment)
        [Column("DatabaseID")]
        public int DatabaseID { get; set; }

        // The year level chosen for the merit. It is represented by a string 
        [Column("YearLevel")]
        public string Yearlevel { get; set; }
    }
}
