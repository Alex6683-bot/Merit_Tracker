using Merit_Tracker.Database;
using Merit_Tracker.Models;

namespace Merit_Tracker.Interfaces
{
    public interface IPageHelper
    {
        public UserModel currentUser { get; set; }
    }
}
