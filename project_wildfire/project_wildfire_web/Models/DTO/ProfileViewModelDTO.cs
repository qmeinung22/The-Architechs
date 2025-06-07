using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using project_wildfire_web.Models;
using project_wildfire_web.Models.DTO;



namespace project_wildfire_web.Models.DTO
{
    public partial class ProfileViewModelDTO
    {
        [Required]
        [Name("UserInfo")]
        public UserInfoDTO UserInfo { get; set; }

        [Name("Location")]
        public string? Location { get; set; }

        [Name("SavedLocations")]
        public ICollection<UserLocationDTO> SavedLocations { get; set; } = new List<UserLocationDTO>();

        [Name("FireSubscriptions")]
        public ICollection<Fire> FireSubscriptions { get; set; } = new List<Fire>();

    }
}


namespace project_wildfire_web.ExtensionsMethods
{
    public static class ProfileViewModelDtoExtensions
    {
        public static ProfileViewModelDTO ToProfileViewModelDTO (this ProfileViewModel pvm)
        {   
            if (pvm.UserInfo.Email == null)
            {
                pvm.UserInfo.Email = string.Empty;
            }
            return new ProfileViewModelDTO
            {
                UserInfo = pvm.UserInfo.ToUserInfoDTO(),
                Location = pvm.Location,
                SavedLocations = pvm.SavedLocations.Select(ul => ul.ToUserLocationDTO()).ToList(),
                // FireSubscriptions = pvm.FireSubscriptions.Select(f => f.ToFireDTO()).ToList()
                // ...existing code...
                FireSubscriptions = pvm.FireSubscriptions.ToList()
// ...existing code...
            };

        }

        public static ProfileViewModel ToProfileViewModel (this ProfileViewModelDTO pvm)
        {
            if (pvm == null)
            {
                throw new ArgumentNullException(nameof(pvm), "UserLocationDTO cannot be null.");
            }
            if (string.IsNullOrEmpty(pvm.UserInfo.UserId))
            {
                throw new ArgumentException("UserId cannot be null or empty.", nameof(pvm.UserInfo.UserId));
            }

            var userInfo = new UserInfo
            {
                UserId = pvm.UserInfo.UserId,
                FirstName = pvm.UserInfo.FirstName,
                LastName = pvm.UserInfo.LastName,
                Email = pvm.UserInfo.Email ?? string.Empty, // Ensure Email is not null
                PhoneNumber = pvm.UserInfo.PhoneNumber
            };

            return new ProfileViewModel
            {
                UserInfo = userInfo,
                Location = pvm.Location,
                SavedLocations = pvm.SavedLocations.Select(ul => ul.ToUserLocation()).ToList(),
                //FireSubscriptions = pvm.FireSubscriptions.Select(f => f.ToFire()).ToList()
                // ...existing code...
                FireSubscriptions = pvm.FireSubscriptions.ToList()
// ...existing code...
            };
        }
    }

}