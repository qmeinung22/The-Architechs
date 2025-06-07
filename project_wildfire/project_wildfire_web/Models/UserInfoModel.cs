namespace project_wildfire_web.Models;
public class UserInfo
{
    public required string UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string FontSize { get; set; } = "medium";
    public bool ContrastMode { get; set; } = false;
    public bool TextToSpeech { get; set; } = false;
}
