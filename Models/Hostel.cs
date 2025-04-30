using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Hostel
    {
        public Hostel()
        {
            // Initialize collections
            Rooms = new HashSet<Room>();
            Amenities = new HashSet<Amenity>();
            Announcements = new HashSet<Announcement>();
        }

        [Key]
        public int HostelId { get; set; }
        
        [Display(Name = "Hostel Code")]
        public string? HostelCode { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        public int Capacity { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Display(Name = "YouTube Video ID")]
        [RegularExpression(@"^[a-zA-Z0-9_-]{11}$", ErrorMessage = "Please enter a valid YouTube video ID (e.g., dQw4w9WgXcQ)")]
        public string? YouTubeVideoId { get; set; }

        [Required]
        public ManagementType ManagementType { get; set; } = ManagementType.InstitutionManaged;

        // Warden information
        public string? WardenName { get; set; }
        public string? WardenContact { get; set; }
        public string? WardenId { get; set; }

        // Landlord information (for privately managed hostels)
        public string? LandlordId { get; set; }

        public string? ImageUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DistanceFromCampus { get; set; }

        // Navigation property
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Amenity> Amenities { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Mixed
    }

    public enum ManagementType
    {
        InstitutionManaged,
        PrivatelyManaged,
        InstitutionallyManaged = InstitutionManaged  // Alias for compatibility
    }
}