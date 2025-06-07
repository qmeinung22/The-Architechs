
namespace project_wildfire_web.Models;
public class ProfileViewModel
{
    public UserInfo? UserInfo { get; set; }
    public string? Location { get; set; }

    // SavedLocations has its own table, so it will need to be populated elsewhere
    public ICollection<UserLocation> SavedLocations { get; set; } = [];
    public required ICollection<Fire> FireSubscriptions { get; set; }

}