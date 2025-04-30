using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [Required]
        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        public required string PostedBy { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? HostelId { get; set; }

        public bool IsActive { get; set; } = true;
        
        // Added IsUrgent property
        public bool IsUrgent { get; set; } = false;

        // Navigation property - Optional relationship to specific hostel
        [ForeignKey("HostelId")]
        public virtual Hostel? Hostel { get; set; }
    }
}