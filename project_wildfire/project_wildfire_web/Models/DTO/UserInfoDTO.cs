using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using project_wildfire_web.Models;
using project_wildfire_web.Models.DTO;

namespace project_wildfire_web.Models.DTO {

    public class UserInfoDTO
    {
        [Required]
        [Name("UserId")]
        public required string UserId { get; set; }

        [Name("FirstName")]
        public string? FirstName { get; set; }

        [Name("LastName")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [Name("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        // User preference properties
        [Name("FontSize")]
        public string FontSize { get; set; } = "medium";

        [Name("ContrastMode")]
        public bool ContrastMode { get; set; } = false;

        [Name("TextToSpeech")]
        public bool TextToSpeech { get; set; } = false;

        public UserInfoDTO() { }

        public UserInfoDTO(string userId, string? firstName, string? lastName, string? email, string? phoneNumber, 
            string fontSize = "medium", bool contrastMode = false, bool textToSpeech = false)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            FontSize = fontSize;
            ContrastMode = contrastMode;
            TextToSpeech = textToSpeech;
        }

    }
}


namespace project_wildfire_web.ExtensionsMethods {

    public static class UserInfoDtoExtensions
    {
        public static UserInfoDTO ToUserInfoDTO (this UserInfo uf)
        {
            return new UserInfoDTO
            {
                UserId = uf.UserId,
                FirstName = uf.FirstName,
                LastName = uf.LastName,
                Email = uf.Email,
                PhoneNumber = uf.PhoneNumber,
                FontSize = uf.FontSize,
                ContrastMode = uf.ContrastMode,
                TextToSpeech = uf.TextToSpeech
            };
        }
    }
    public static class UserInfoExtensions
    {

        public static UserInfo ToUserInfo (this UserInfoDTO uinfo)
        {
            return new UserInfo
            {
                UserId = uinfo.UserId,
                FirstName = uinfo.FirstName,
                LastName = uinfo.LastName,
                Email = uinfo.Email,
                PhoneNumber = uinfo.PhoneNumber,
                FontSize = uinfo.FontSize,
                ContrastMode = uinfo.ContrastMode,
                TextToSpeech = uinfo.TextToSpeech
            };
        }
    }
}