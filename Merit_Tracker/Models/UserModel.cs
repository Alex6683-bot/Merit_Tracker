using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Merit_Tracker.Models
{
    // Enum representing the role of a user in the system
    public enum UserRole
    {
        Admin = 1,    // Administrator with full permissions
        Teacher       // Regular teacher user with limited access
    }

    // Model representing a user account, stored in the "users" table
    [Table("users")]
    public class UserModel
    {
        // Username used to log in to the system
        [Column("Username")]
        public string UserName { get; set; }

        // User's password (assumed to be stored hashed in production)
        [Column("Password")]
        public string Password { get; set; }

        // Primary key identifier for the user
        [Key]
        public int ID { get; set; }

        // First name of the user
        public string FirstName { get; set; }

        // Last name of the user
        public string LastName { get; set; }

        // Role assigned to the user (Admin or Teacher)
        public UserRole Role { get; set; }
    }
}
