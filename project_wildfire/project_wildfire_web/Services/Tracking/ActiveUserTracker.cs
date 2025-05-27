using System.Collections.Generic;

namespace project_wildfire_web.Services.Tracking
{

    public class ActiveUserTracker : IActiveUserTracker
    {
        private readonly HashSet<string> _activeUsers = new();

        public void AddUser(string userId)
        {
            _activeUsers.Add(userId);
        }

        public void RemoveUser(string userId)
        {
            _activeUsers.Remove(userId);
        }

        public IReadOnlyCollection<string> GetActiveUserIds() => _activeUsers;
    }
}