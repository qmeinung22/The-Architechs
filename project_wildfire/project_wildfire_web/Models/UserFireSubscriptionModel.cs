using project_wildfire_web.Models;

namespace project_wildfire_web.Models;

public class UserFireSubscription
{
    public User User {get; set;}
    public string UserId {get; set;}
    public string FireId {get; set;}
    public Fire Fire { get; set; }
}