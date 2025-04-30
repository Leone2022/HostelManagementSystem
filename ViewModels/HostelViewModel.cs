using HostelMS.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class HostelViewModel
    {
        public int HostelId { get; set; }

        [Display(Name = "Hostel Code")]
        public string? HostelCode { get; set; }

        [Required(ErrorMessage = "Hostel name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Hostel Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "YouTube Video ID")]
        [StringLength(20, ErrorMessage = "YouTube Video ID cannot be longer than 20 characters")]
        public string? YouTubeVideoId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot be longer than 100 characters")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender assignment is required")]
        [Display(Name = "Gender Assignment")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Management type is required")]
        [Display(Name = "Management Type")]
        public ManagementType ManagementType { get; set; } = ManagementType.InstitutionManaged;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        [Display(Name = "Total Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Warden Name")]
        [StringLength(100, ErrorMessage = "Warden name cannot be longer than 100 characters")]
        public string WardenName { get; set; } = string.Empty;

        [Display(Name = "Warden Contact")]
        [StringLength(20, ErrorMessage = "Contact number cannot be longer than 20 characters")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string WardenContact { get; set; } = string.Empty;

        [Display(Name = "Distance from Campus (km)")]
        [Range(0, 100, ErrorMessage = "Distance must be between 0 and 100 km")]
        public decimal DistanceFromCampus { get; set; }

        [Display(Name = "Hostel Image")]
        public IFormFile? ImageFile { get; set; }

        // Interior images
        [Display(Name = "Interior Images")]
        public List<IFormFile>? InteriorImages { get; set; }

        // Services
        [Display(Name = "Hostel Services")]
        public List<string>? HostelServices { get; set; }

        // Amenities
        [Display(Name = "Amenity Names")]
        public List<string>? AmenityNames { get; set; }

        [Display(Name = "Amenity Descriptions")]
        public List<string>? AmenityDescriptions { get; set; }

        [Display(Name = "Current Image")]
        public string? ExistingImagePath { get; set; }
    }
}