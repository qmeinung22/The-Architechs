public interface IActiveUserTracker
{
    void AddUser(string userId);
    void RemoveUser(string userId);
    IReadOnlyCollection<string> GetActiveUserIds();
}
