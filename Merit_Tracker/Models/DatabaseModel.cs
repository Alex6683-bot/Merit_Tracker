using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit_Tracker.Models
{
    // This is the model used to represent databases of users 
    [Table("Databases")]
    public class DatabaseModel
    {
        [Key]
        public int Id { get; set; } // This is used as a key for the database, it is not used for anything

        [Column("Name")]
        public string Name { get; set; }

        [Column("DatabaseID")]
        public int DatabaseID { get; set; } // This is the ID used to reference to a specfic database

        [Column("UserID")]
        public int UserID { get; set; } // This id refers to user ids that can access this database

    }
}
