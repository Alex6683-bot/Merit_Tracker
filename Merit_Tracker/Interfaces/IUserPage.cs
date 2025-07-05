using Merit_Tracker.Database;
using Merit_Tracker.Models;

namespace Merit_Tracker.Interfaces
{
    public interface IUserPage
    {
        public UserModel currentUser { get; set; }
    }
}
