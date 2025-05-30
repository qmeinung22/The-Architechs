using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using project_wildfire_web.Models;
using project_wildfire_web.DAL.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using project_wildfire_web.Models.DTO;


namespace project_wildfire_web.Controllers;

[ApiController]
[Route("api/User")]
public class UserApiController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<WildfireAPIController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public UserApiController(
        ILogger<WildfireAPIController> logger, 
        IUserRepository userRepository,
        UserManager<IdentityUser> userManager
        )
    {
        _userRepository = userRepository;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> SaveModalEdits([FromBody]UserInfo userInfo)
    {
        // Find auth and primary user records for the given ID
        var user = await _userRepository.GetUserByIdAsync(userInfo.UserId);
        var authUser = await _userManager.FindByIdAsync(userInfo.UserId);

        // Null check
        if (user == null || authUser == null)
        {
            return NotFound("User record not found.");
        }

        // Update primary user record
        user.FirstName = userInfo.FirstName;
        user.LastName = userInfo.LastName;

        //update custom preferences record
        user.FontSize = userInfo.FontSize ?? "medium"; //Added the .FontSize class Attribute 
        user.ContrastMode = userInfo.ContrastMode;
        user.TextToSpeech = userInfo.TextToSpeech;

        // Update auth user record
        authUser.Email = userInfo.Email;
        authUser.PhoneNumber = userInfo.PhoneNumber;

        // Save changes
        _userRepository.UpdateUser(user);
        await _userManager.UpdateAsync(authUser);

        // Return updated user
        return Ok();
    }

    public class UpdateEmailRequest
    {
        public string NewEmail { get; set; }
    }

    [HttpPost("UpdateEmail")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.NewEmail))
        {
            return BadRequest(new { message = "Email cannot be empty." });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var setEmailResult = await _userManager.SetEmailAsync(user, request.NewEmail);
        if (!setEmailResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to update email", errors = setEmailResult.Errors });
        }

        var setUserNameResult = await _userManager.SetUserNameAsync(user, request.NewEmail);
        if (!setUserNameResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to update username", errors = setUserNameResult.Errors });
        }

        return Ok(new { message = "Email updated successfully" });
    }
 
    //Saves Preferences 
    [HttpPost("preferences")]
    public async Task<IActionResult> SavePreferences([FromBody] PreferencesDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation($"🔍 Incoming savePreferences call for user: {userId}");
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("⚠️ No user ID found in claims.");
            return Unauthorized("User not authenticated");
        }
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"⚠️ No matching user found in DB for userId: {userId}");
            return NotFound("User not found");
        }

        _logger.LogInformation($"💾 Saving preferences: FontSize={dto.FontSize}, ContrastMode={dto.ContrastMode}, TextToSpeech={dto.TextToSpeech}");
        user.FontSize = dto.FontSize ?? "medium";
        user.ContrastMode = dto.ContrastMode;
        user.TextToSpeech = dto.TextToSpeech;

        _userRepository.UpdateUser(user);
        _logger.LogInformation("✅ Preferences updated and saved to DB");
        return Ok();
    }

}





