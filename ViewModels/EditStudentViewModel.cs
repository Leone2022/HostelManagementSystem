using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class EditStudentViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

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

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
        
        public string? ExistingProfilePicture { get; set; }
    }
}