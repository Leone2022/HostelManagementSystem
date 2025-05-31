using HostelMS.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HostelMS.ViewModels
{
    public class HostelViewModel
    {
        public int HostelId { get; set; }

        [Required(ErrorMessage = "Hostel name is required")]
        [StringLength(100, ErrorMessage = "Hostel name cannot exceed 100 characters")]
        [Display(Name = "Hostel Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hostel code is required")]
        [StringLength(10, ErrorMessage = "Hostel code cannot exceed 10 characters")]
        [Display(Name = "Hostel Code")]
        public string HostelCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please select management type")]
        [Display(Name = "Management Type")]
        public ManagementType ManagementType { get; set; }

        [Required(ErrorMessage = "Please select gender type")]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Contact information is required")]
        [StringLength(200, ErrorMessage = "Contact information cannot exceed 200 characters")]
        [Display(Name = "Contact Information")]
        public string ContactInfo { get; set; } = string.Empty;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Total Capacity")]
        [Range(1, int.MaxValue, ErrorMessage = "Total capacity must be at least 1")]
        public int TotalCapacity { get; set; }

        [Display(Name = "Current Occupancy")]
        [Range(0, int.MaxValue, ErrorMessage = "Current occupancy cannot be negative")]
        public int CurrentOccupancy { get; set; }

        [Display(Name = "Number of Rooms")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be at least 1")]
        public int NumberOfRooms { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        [Range(0, 100, ErrorMessage = "Distance must be between 0 and 100 km")]
        [Display(Name = "Distance From Campus (km)")]
        public decimal DistanceFromCampus { get; set; }

        [StringLength(100, ErrorMessage = "Warden name cannot exceed 100 characters")]
        [Display(Name = "Warden Name")]
        public string? WardenName { get; set; }

        [StringLength(20, ErrorMessage = "Warden contact cannot exceed 20 characters")]
        [Display(Name = "Warden Contact")]
        public string? WardenContact { get; set; }

        [Display(Name = "Facilities")]
        [StringLength(1000, ErrorMessage = "Facilities description cannot exceed 1000 characters")]
        public string? Facilities { get; set; }

        [Display(Name = "Rules and Regulations")]
        [StringLength(2000, ErrorMessage = "Rules cannot exceed 2000 characters")]
        public string? Rules { get; set; }

        [Display(Name = "Emergency Contact")]
        [StringLength(100, ErrorMessage = "Emergency contact cannot exceed 100 characters")]
        public string? EmergencyContact { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }

        // Image upload properties
        [Display(Name = "Hostel Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Current Image")]
        public string? ExistingImagePath { get; set; }

        // YouTube video
        [StringLength(50, ErrorMessage = "YouTube Video ID cannot exceed 50 characters")]
        [Display(Name = "YouTube Video ID")]
        public string? YouTubeVideoId { get; set; }

        // Quick setup properties
        [Display(Name = "Enable Quick Room Setup")]
        public bool EnableQuickSetup { get; set; }

        [Display(Name = "Number of Floors")]
        [Range(1, 20, ErrorMessage = "Number of floors must be between 1 and 20")]
        public int? FloorCount { get; set; }

        [Display(Name = "Rooms Per Floor")]
        [Range(1, 50, ErrorMessage = "Rooms per floor must be between 1 and 50")]
        public int? RoomsPerFloor { get; set; }

        [Display(Name = "Default Room Type")]
        public RoomType? DefaultRoomType { get; set; }

        [Display(Name = "Default Room Capacity")]
        [Range(1, 6, ErrorMessage = "Default capacity must be between 1 and 6")]
        public int? DefaultCapacity { get; set; }

        // Navigation properties for display
        public List<RoomViewModel>? Rooms { get; set; }
        public List<BookingCreateViewModel>? RecentBookings { get; set; }

        // Calculated properties
        public int AvailableCapacity => TotalCapacity - CurrentOccupancy;
        public double OccupancyRate => TotalCapacity > 0 ? (double)CurrentOccupancy / TotalCapacity * 100 : 0;
        public bool IsFullyOccupied => CurrentOccupancy >= TotalCapacity;
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string OccupancyDisplay => $"{CurrentOccupancy}/{TotalCapacity} ({OccupancyRate:F1}%)";
        public string GenderDisplay => Gender.ToString();
        public string ManagementTypeDisplay => ManagementType.ToString().Replace("_", " ");
    }
}