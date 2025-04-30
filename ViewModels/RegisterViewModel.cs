using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; } = string.Empty;

        [Display(Name = "Course")]
        public string? Course { get; set; }

        [Display(Name = "Year")]
        public string? Year { get; set; }

        [Display(Name = "Parent Name")]
        public string? ParentName { get; set; }

        [Display(Name = "Parent Contact")]
        [Phone]
        public string? ParentContact { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Nationality")]
        public string? Nationality { get; set; }

        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [Display(Name = "Emergency Contact Phone")]
        [Phone]
        public string? EmergencyContactPhone { get; set; }

        // Add profile picture property
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}