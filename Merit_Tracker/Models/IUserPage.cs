using Merit_Tracker.Database;

namespace Merit_Tracker.Models
{
    public interface IUserPage
    {
        public UserModel currentUser { get; set; }
    }
}
