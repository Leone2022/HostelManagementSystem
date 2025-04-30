using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ResolvedAt { get; set; }

        public string? Resolution { get; set; }

        public string? StaffNotes { get; set; }

        public bool IsUrgent { get; set; } = false;

        // User who reported the issue
        public string? ReportedById { get; set; }

        [ForeignKey("ReportedById")]
        public virtual ApplicationUser? ReportedBy { get; set; }

        // User who resolved the issue
        public string? ResolvedById { get; set; }

        // Navigation properties
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }
    }
}