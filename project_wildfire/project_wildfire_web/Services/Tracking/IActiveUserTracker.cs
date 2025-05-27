using System.Collections.Generic;

namespace project_wildfire_web.Services.Tracking
{

    public interface IActiveUserTracker
    {
        void AddUser(string userId);
        void RemoveUser(string userId);
        IReadOnlyCollection<string> GetActiveUserIds();
    }

}