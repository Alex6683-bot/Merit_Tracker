using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit_Tracker.Models
{
    [Table("users")]
    public class UserModel
    {
        [Column("Username")]
        public string UserName { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Key]
        public int ID { get; set; }

        public int UserId { get; set; }

        public string Role { get; set; }
    }
}
