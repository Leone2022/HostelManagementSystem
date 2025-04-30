using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMS.Models
{
    public enum NotificationType
    {
        General,
        StudentApproval,
        RoomAssignment,
        MaintenanceRequest,
        SystemAlert
    }

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        // Link to relevant page (e.g., student profile, room details)
        [StringLength(255)]
        public string? Link { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ReadAt { get; set; }

        public bool IsRead { get; set; } = false;

        public NotificationType Type { get; set; } = NotificationType.General;

        // Target users - can be null to target all admin/staff
        public string? TargetUserId { get; set; }

        // Recipient - who should receive this notification
        public string RecipientId { get; set; } = string.Empty;

        // Sender - can be null for system notifications
        public string? SenderUserId { get; set; }

        public string? SenderName { get; set; }
    }
}